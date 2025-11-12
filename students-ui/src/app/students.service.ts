import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Student { id: number; name: string; dept: string; }

@Injectable({ providedIn: 'root' })
export class StudentsService {
  // use relative path so browser calls localhost:8080 and nginx can proxy to api
  private base = '/api/students';

  constructor(private http: HttpClient) {}

  list(): Observable<Student[]> { return this.http.get<Student[]>(this.base); }
  get(id: number): Observable<Student> { return this.http.get<Student>(`${this.base}/${id}`); }
  create(s: Omit<Student, 'id'>): Observable<Student> { return this.http.post<Student>(this.base, s); }
  update(s: Student): Observable<void> { return this.http.put<void>(`${this.base}/${s.id}`, s); }
  remove(id: number): Observable<void> { return this.http.delete<void>(`${this.base}/${id}`); }
} 
