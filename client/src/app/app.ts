import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { environment } from '../environments/environment';
import { User } from '../types/user';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {
  protected readonly title = signal('client');
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;
  protected testData = signal<User | null>(null);


  ngOnInit(): void {
    this.http.get<User>(this.baseUrl + 'account/test').subscribe({
      next: (data) => this.testData.set(data as User),
      error: (error) => console.error('API Error:', error)
    });
  }



}
