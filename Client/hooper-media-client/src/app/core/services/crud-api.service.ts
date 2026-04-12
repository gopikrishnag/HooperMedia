import { inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { API_BASE_URL } from '../config/api.config';

export abstract class CrudApiService<TEntity, TCreate, TUpdate, TId extends number | string> {
  protected readonly http = inject(HttpClient);
  protected readonly apiBaseUrl = inject(API_BASE_URL);

  protected abstract readonly resourcePath: string;

  protected get resourceUrl(): string {
    return `${this.apiBaseUrl}/${this.resourcePath}`;
  }

  list(): Observable<TEntity[]> {
    return this.http.get<TEntity[]>(this.resourceUrl);
  }

  getById(id: TId): Observable<TEntity> {
    return this.http.get<TEntity>(`${this.resourceUrl}/${id}`);
  }

  create(payload: TCreate): Observable<TEntity> {
    return this.http.post<TEntity>(this.resourceUrl, payload);
  }

  update(id: TId, payload: TUpdate): Observable<TEntity> {
    return this.http.put<TEntity>(`${this.resourceUrl}/${id}`, payload);
  }

  delete(id: TId): Observable<void> {
    return this.http.delete<void>(`${this.resourceUrl}/${id}`);
  }
}