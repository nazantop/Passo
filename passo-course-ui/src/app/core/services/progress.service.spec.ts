import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ProgressService } from './progress.service';
import { environment } from '../../../environments/environment';

describe('ProgressService', () => {
  let svc: ProgressService;
  let http: HttpTestingController;
  const base = `${environment.apiBaseUrl}/progress`;

  beforeEach(() => {
    TestBed.configureTestingModule({ imports: [HttpClientTestingModule] });
    svc = TestBed.inject(ProgressService);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('getMine returns data', () => {
    let pct = 0;
    svc.getMine().subscribe(r => (pct = r[0].percent));
    const req = http.expectOne(`${base}/me`);
    expect(req.request.method).toBe('GET');
    req.flush([{ courseId: 'c1', completedLessons: 1, completedQuizzes: 0, totalLessons: 2, totalQuizzes: 0, percent: 50 }]);
    expect(pct).toBe(50);
  });

  it('completeLesson posts', () => {
    let ok = false;
    svc.completeLesson('l1').subscribe(() => (ok = true));
    const req = http.expectOne(`${base}/lessons/l1/complete`);
    expect(req.request.method).toBe('POST');
    req.flush({});
    expect(ok).toBeTrue();
  });

  it('completeQuiz posts', () => {
    let ok = false;
    svc.completeQuiz('c1', 2).subscribe(() => (ok = true));
    const req = http.expectOne(`${base}/quizzes/c1/2/complete`);
    expect(req.request.method).toBe('POST');
    req.flush({});
    expect(ok).toBeTrue();
  });
});