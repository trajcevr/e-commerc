import { HttpClient } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-test-error',
  standalone: true,
  imports: [MatButtonModule],
  templateUrl: './test-error.component.html',
  styleUrls: ['./test-error.component.scss']
})
export class TestErrorComponent {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);
  validationErrors?: string[];

  get404Error() {
    this.http.get(this.baseUrl + "buggy/notfound").subscribe({
      next: response => console.log(response),
      error: error => console.error('404 Error:', error)
    });
  }

  get400Error() {
    this.http.get(this.baseUrl + "buggy/badrequest").subscribe({
      next: response => console.log(response),
      error: error => console.error('400 Error:', error)
    });
  }

  get401Error() {
    this.http.get(this.baseUrl + "buggy/unauthorized").subscribe({
      next: response => console.log(response),
      error: error => console.error('401 Error:', error)
    });
  }

  get500Error() {
    this.http.get(this.baseUrl + "buggy/internalerror").subscribe({
      next: response => console.log(response),
      error: error => console.error('500 Error:', error)
    });
  }

  get400ValidationError() {
    this.http.post(this.baseUrl + "buggy/validationerror", {}).subscribe({
      next: response => console.log(response),
      error: error => this.validationErrors = error
    });
  }
}
