import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { ProgressResponse } from '../models/progress.models';
import { Observable } from 'rxjs';


@Injectable({ providedIn: 'root' })
export class ProgressService {

  private base = `${environment.apiBaseUrl}/progress`;

  constructor(private http: HttpClient) {}
  
  getMine(): Observable<ProgressResponse[]> {
    return this.http.get<ProgressResponse[]>(`${this.base}/me`);
  }

  incrementLesson(courseId: string) {
    return this.http.post<void>(`${this.base}/lesson-completed`, { courseId });
  }

  incrementQuiz(courseId: string) {
    return this.http.post<void>(`${this.base}/quiz-completed`, { courseId });
  }

  completeLesson(lessonId: string) {
    return this.http.post<void>(`${this.base}/lessons/${lessonId}/complete`, {});
  }
  completeQuiz(courseId: string, quizIndex: number) {
    return this.http.post<void>(`${this.base}/quizzes/${courseId}/${quizIndex}/complete`, {});
  }
}