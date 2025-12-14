import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { LoginCreds, RegisterCreds, User } from '../../types/user';
import { tap } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  // http请求
  private http = inject(HttpClient);
  // 环境变量里获取访问路径
  private baseUrl = environment.apiUrl;
  currentUser = signal<User | null>(null);

  //#region http请求
  // 注册
  register(creds : RegisterCreds){
    return this.http.post<User>(this.baseUrl + 'account/register',creds).pipe(
      tap(user => {
        if (user){
          this.setCurrentUser(user);
        }
      })
    );
  }
  

  // 登录
  login(creds: LoginCreds){
    return this.http.post<User>(this.baseUrl + 'account/login',creds).pipe(
      tap(user =>{
        if(user){
          this.setCurrentUser(user);
        }
      })
    );
  }
  // 登出
  logout(){
    return this.http.post(this.baseUrl + 'account/logout',{}).subscribe({
      next: () =>{
        this.currentUser.set(null);
        localStorage.removeItem('user')
      }
    })
  }
  //#endregion

  //#region 公用方法
  setCurrentUser(user: User) {
    localStorage.setItem('user',JSON.stringify(user));
    this.currentUser.set(user);
  }
  //#endregion
}
