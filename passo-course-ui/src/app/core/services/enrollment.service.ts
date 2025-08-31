import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { EnrollmentRequest, EnrollmentResponse } from '../models/enrollment.models';
import { CourseResponse } from '../models/course.models';
import { map, Observable } from 'rxjs';


@Injectable({ providedIn: 'root' })
export class EnrollmentService {
  private base = `${environment.apiBaseUrl}/enrollments`;

  constructor(private http: HttpClient) {}

  enroll(req: EnrollmentRequest){ 
    return this.http.post<EnrollmentResponse>(this.base, req); 
  }

  myEnrollments(){ 
    return this.http.get<CourseResponse[]>(`${this.base}/me`); 
  }

  unenroll(courseId: string) {
    return this.http.delete<void>(`${this.base}/${courseId}`);
  }
  
  isEnrolled(courseId: string): Observable<boolean> {
    return this.myEnrollments().pipe(
      map(courses => courses.some(c => c.id === courseId))
    );
  }
}