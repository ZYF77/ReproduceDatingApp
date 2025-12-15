import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { ToastService } from '../services/toast-service';
import { NavigationExtras, Router } from '@angular/router';
import { catchError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {

  //函数式编程
  const toast = inject(ToastService);
  const router = inject(Router);


  return next(req).pipe(
    // catchError捕获所有 HTTP 响应错误
    catchError(error => {
      if(error){
        switch(error.status){
          case 400:
            if(error.error.errors){ //判断这是否是一个 ASP.NET Core 的模型验证错误。
              const modelStateErrors = [];
              for (const key in error.error.errors) {
                if (error.error.errors[key]) {
                  modelStateErrors.push(error.error.errors[key]);
                }
              }
              throw modelStateErrors.flat();
            }
            else{
              toast.error(error.error);
            }
            break;
          case 401:
            toast.error("Unauthorized");
            break;
          case 404:
            toast.error("NotFound");
            router.navigateByUrl('/not-found');
            break;
          case 500:
            const navigationExtras: NavigationExtras = {state: {error: error.error}}; //隐式、不通过 URL 显示,通过内存的传参方式 :路由传参
            router.navigateByUrl('/server-error',navigationExtras);
            break;
          default:
            toast.error("Something went wrong");
            break;
        }
      }
      throw error;
    })
  );
};
