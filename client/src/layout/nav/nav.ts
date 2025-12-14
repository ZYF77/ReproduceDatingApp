import { Component, inject, Signal, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink, RouterLinkActive } from "@angular/router";
import { AccountService } from '../../core/services/account-service';
import { themes } from '../theme';
import { BusyService } from '../../core/services/busy-service';

@Component({
  selector: 'app-nav',
  imports: [RouterLink, RouterLinkActive, FormsModule],
  templateUrl: './nav.html',
  styleUrl: './nav.css',
})
export class Nav {

  protected selectedTheme = signal<string>(localStorage.getItem('theme') || 'light');
  protected themes = themes;

  protected accountService = inject(AccountService);
  protected busyService = inject(BusyService);
  protected router = inject(Router);
  protected creds: any = {};

  protected loading = signal(false); //登录中




  handleSelectedTheme(theme: string) {
  this.selectedTheme.set(theme);
  localStorage.setItem('theme',theme);
  // 切换全局主题
  document.documentElement.setAttribute('data-theme', theme);
  //选择后去除焦点，关闭下拉框。(先获取到焦点，在移除)
  const elem = document.activeElement as HTMLElement;
  if(elem){
    elem.blur();
  }
  }

  login() {
    this.loading.set(true);
    this.accountService.login(this.creds).subscribe({
      next: result => {
        this.router.navigateByUrl("/members");
        this.creds = {}
      },
      error: error => {
        console.log(error.error);
      },
      complete: () => this.loading.set(false)
    })
  };

  logout() {
    this.accountService.logout();
  }
}
