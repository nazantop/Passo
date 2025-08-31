import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { CourseService } from './course.service';
import { environment } from '../../../environments/environment';

describe('CourseService', () => {
  let svc: CourseService;
  let http: HttpTestingController;
  const base = `${environment.apiBaseUrl}/courses`;

  beforeEach(() => {
    TestBed.configureTestingModule({ imports: [HttpClientTestingModule] });
    svc = TestBed.inject(CourseService);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('getAll', () => {
    let len = 0;
    svc.getAll().subscribe(r => (len = r.length));
    const req = http.expectOne(base);
    expect(req.request.method).toBe('GET');
    req.flush([{ id: 'a' }, { id: 'b' }]);
    expect(len).toBe(2);
  });

  it('getById', () => {
    let id = '';
    svc.getById('c1').subscribe(r => (id = r.id));
    const req = http.expectOne(`${base}/c1`);
    expect(req.request.method).toBe('GET');
    req.flush({ id: 'c1' });
    expect(id).toBe('c1');
  });

  it('getOutline', () => {
    let cid = '';
    svc.getOutline('c1').subscribe(r => (cid = r.id ?? r.id));
    const req = http.expectOne(`${base}/c1/outline`);
    expect(req.request.method).toBe('GET');
    req.flush({ id: 'c1' });
    expect(cid).toBe('c1');
  });

  it('create', () => {
    let created = '';
    const body = { title: 'T', description: 'D', duration: 10, difficulty: 0 };
    svc.create(body as any).subscribe((r: any) => (created = r.id));
    const req = http.expectOne(base);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(body);
    req.flush({ id: 'new' });
    expect(created).toBe('new');
  });

  it('update', () => {
    let ok = false;
    const body = { title: 'Tx' };
    svc.update('c1', body as any).subscribe(() => (ok = true));
    const req = http.expectOne(`${base}/c1`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual(body);
    req.flush({});
    expect(ok).toBeTrue();
  });

  it('delete', () => {
    let ok = false;
    svc.delete('c1').subscribe(() => (ok = true));
    const req = http.expectOne(`${base}/c1`);
    expect(req.request.method).toBe('DELETE');
    req.flush({});
    expect(ok).toBeTrue();
  });
});
