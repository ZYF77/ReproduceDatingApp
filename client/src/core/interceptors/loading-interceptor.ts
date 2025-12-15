import { HttpEvent, HttpInterceptorFn, HttpParams } from '@angular/common/http';
import { inject } from '@angular/core';
import { BusyService } from '../services/busy-service';
import { delay, finalize, identity, of, tap } from 'rxjs';
import { environment } from '../../environments/environment';

const cache = new Map<string,HttpEvent<unknown>>();

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  const busyService = inject(BusyService);

  const generateCacheKey = (url: string, params: HttpParams): string => {
    const paramsString = params.keys().map(key => `${key} = ${params.get(key)}`).join('&');
    return paramsString ? `${url}?${paramsString}` : url;
  }


  //缓存失效
  const invalidationCache = (urlPattern: string) => {
    for (const key of cache.keys()) {
      if (key.includes(urlPattern)) {
        cache.delete(key); //清除指定缓存
        console.log(`Cache invalidated for: ${key}`);
      }
    }
  }


  const cacheKey = generateCacheKey(req.url, req.params);//防止分页变更时仍然获取原先缓存

  if (req.method.includes('POST') && req.url.includes('/likes')) {
    invalidationCache('/likes');
  }

  if (req.method.includes('POST') && req.url.includes('/messages')) {
    invalidationCache('/messages');
  }

  if (req.method.includes('DELETE') && req.url.includes('/messages')) {
    invalidationCache('/messages');
  }
  if (req.method.includes('POST') && req.url.includes('/logout')) {
     cache.clear(); //清除所有缓存
  }


  if (req.method === 'GET') {
    const cachedResponse = cache.get(cacheKey);
    if (cachedResponse) {
      busyService.idle(); //因为没有经过下游的处理器，所以不会调用 finalize 里的 idle 方法，所以这里手动调用一次
      return of(cachedResponse); //创建一个立即发射缓存响应的 Observable，并直接返回它。of() 是同步的，所以组件会瞬间拿到数据。（读取缓存）
    }
  }
  busyService.busy();

  return next(req).pipe(
    // (environment.production ? identity : delay(500)),//identity是函数，不给他参数什么都不会执行
    tap(response => {
      if (req.method === 'GET') {
        cache.set(cacheKey, response); //存入缓存
      }
    }),
    finalize(() => busyService.idle()) //这个回调函数无论 Observable 是正常 complete（请求成功）还是 error（请求失败），都必定会被执行。
  );
};
