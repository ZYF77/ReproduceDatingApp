import { Component, signal } from '@angular/core';

@Component({
  selector: 'app-home',
  imports: [],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home {
  protected registerMode = signal<boolean>(false);
  
  showRegister(value: boolean){
    this.registerMode.set(value);
  }
}
