import { Component, inject, OnInit } from '@angular/core';
import { AccountService } from '../../../core/services/account-service';

@Component({
  selector: 'app-register',
  imports: [],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register implements OnInit {

  private accountService = inject(AccountService);
  ngOnInit(): void {
    throw new Error('Method not implemented.');
  }
  register(){
    
  }

}
