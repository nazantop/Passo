import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { CourseResponse, CourseUpdateRequest } from '../models/course.models';


@Injectable({ providedIn: 'root' })
export class CourseService {
  
  private base = `${environment.apiBaseUrl}/courses`;

  constructor(private http: HttpClient) {}


  getAll(){ 
    return this.http.get<CourseResponse[]>(this.base); 
  }

  getAllByInstructorId(){ 
    return this.http.get<CourseResponse[]>(`${this.base}/my-courses`);
  }

  getById(id: string){ 
    return this.http.get<CourseResponse>(`${this.base}/${id}`);
  }

  create(payload: {
    title: string; description: string; duration: number; difficulty: number;
    totalLessons: number; totalQuizzes: number; lessons: { title: string; order: number }[];
    quizes: { title: string; order: number }[]
    }) 
    {
      return this.http.post<{ id: string }>(`${this.base}`, payload);
  }
  update(id: string, req: CourseUpdateRequest){ 
    return this.http.put<CourseResponse>(`${this.base}/${id}`, req); 
  }

  delete(id: string){ 
    return this.http.delete<void>(`${this.base}/${id}`); 
  } 

  getOutline(courseId: string) {
    return this.http.get<CourseResponse>(`${this.base}/${courseId}/outline`);
  }
}