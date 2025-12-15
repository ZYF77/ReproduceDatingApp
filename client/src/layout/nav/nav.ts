import { Component, inject, OnInit, Signal, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink, RouterLinkActive } from "@angular/router";
import { AccountService } from '../../core/services/account-service';
import { themes } from '../theme';
import { BusyService } from '../../core/services/busy-service';
import { ToastService } from '../../core/services/toast-service';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-nav',
  imports: [RouterLink, RouterLinkActive, FormsModule],
  templateUrl: './nav.html',
  styleUrl: './nav.css',
})
export class Nav implements OnInit{

  protected selectedTheme = signal<string>(localStorage.getItem('theme') || 'light');
  protected themes = themes;

  protected accountService = inject(AccountService);
  protected busyService = inject(BusyService);
  protected toastService = inject(ToastService);
  protected router = inject(Router);
  protected creds: any = {};

  protected loading = signal(false); //登录中


  ngOnInit(): void {
    document.documentElement.setAttribute('data-theme', this.selectedTheme());
  }


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
    this.accountService.login(this.creds).pipe(
      finalize(() =>{
        this.loading.set(false);
      })
    ).subscribe({
      next: result => {
        this.router.navigateByUrl("/members");
        this.toastService.success(`Welcome!${result.displayName} You loging successful!`);
        this.creds = {}
      },
      error: error => {
        this.toastService.error(error.error);
      },
      complete: () => this.loading.set(false)
    })
  };

  logout() {
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }
}
