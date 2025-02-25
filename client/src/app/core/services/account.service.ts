import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Address, User } from '../../shared/models/user';
import { map, tap } from 'rxjs';
//withCredentials using the cookie
@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl = environment.apiUrl;
  httpClient = inject(HttpClient);
  currentUser = signal<User | null>(null);

  login(values: any) {
    let params = new HttpParams();
    params = params.append('useCookies', true);
    return this.httpClient.post<User>(this.baseUrl + "login", values, {params});
  }

  register(values: any) {
    return this.httpClient.post(this.baseUrl + "account/register", values);
  }

  getUserInfo() {
    return this.httpClient.get<User>(this.baseUrl + "account/user-info").pipe(
      map(user => {
        this.currentUser.set(user);
        return user;
      })
    )
  }

  logout() {
    return this.httpClient.post<User>(this.baseUrl + "account/logout", {});
  }

  updateAddress(address: Address) {
    return this.httpClient.post(this.baseUrl + "account/address", address).pipe(
      tap(() => {
        this.currentUser.update(user => {
          if (user) user.address = address;
          return user;
        })
      })
    )
  }

  getAuthState() {
    return this.httpClient.get<{isAuthenticated: boolean}>(this.baseUrl + "account/auth-status");
  }
}
