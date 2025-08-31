import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { EnrollmentService } from './enrollment.service';
import { environment } from '../../../environments/environment';
import { of } from 'rxjs';

describe('EnrollmentService', () => {
  let svc: EnrollmentService;
  let http: HttpTestingController;
  const base = `${environment.apiBaseUrl}/enrollments`;

  beforeEach(() => {
    TestBed.configureTestingModule({ imports: [HttpClientTestingModule] });
    svc = TestBed.inject(EnrollmentService);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('enroll posts body', () => {
    const body = { courseId: 'c1' };
    let ok = false;
    svc.enroll(body).subscribe(() => (ok = true));
    const req = http.expectOne(base);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(body);
    req.flush({ id: 'e1' });
    expect(ok).toBeTrue();
  });

  it('myEnrollments gets list', () => {
    let len = 0;
    svc.myEnrollments().subscribe(r => (len = r.length));
    const req = http.expectOne(`${base}/me`);
    expect(req.request.method).toBe('GET');
    req.flush([{ id: 'a' }, { id: 'b' }]);
    expect(len).toBe(2);
  });

  it('unenroll deletes by id', () => {
    let ok = false;
    svc.unenroll('c1').subscribe(() => (ok = true));
    const req = http.expectOne(`${base}/c1`);
    expect(req.request.method).toBe('DELETE');
    req.flush({});
    expect(ok).toBeTrue();
  });

  it('isEnrolled true when course exists', (done) => {
    spyOn(svc, 'myEnrollments').and.returnValue(of([{ id: 'c42' } as any]));
    svc.isEnrolled('c42').subscribe(v => { expect(v).toBeTrue(); done(); });
  });

  it('isEnrolled false when course missing', (done) => {
    spyOn(svc, 'myEnrollments').and.returnValue(of([{ id: 'x' } as any]));
    svc.isEnrolled('y').subscribe(v => { expect(v).toBeFalse(); done(); });
  });
});