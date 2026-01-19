import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class UserContextService {
  private readonly STORAGE_KEY = 'ats_current_user_id';
  private _currentUserId: string | null = null;

  constructor() {
    this._currentUserId = localStorage.getItem(this.STORAGE_KEY);
  }

  get currentUserId(): string | null {
    return this._currentUserId;
  }

  setCurrentUser(id: string) {
    this._currentUserId = id;
    localStorage.setItem(this.STORAGE_KEY, id);
  }

  clearUser() {
    this._currentUserId = null;
    localStorage.removeItem(this.STORAGE_KEY);
  }
}