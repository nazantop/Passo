import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { AuthService } from './auth.service';
import { environment } from '../../../environments/environment';

describe('AuthService', () => {
  let svc: AuthService;
  let http: HttpTestingController;
  const base = `${environment.apiBaseUrl}/auth`;

  beforeEach(() => {
    TestBed.configureTestingModule({ imports: [HttpClientTestingModule] });
    svc = TestBed.inject(AuthService);
    http = TestBed.inject(HttpTestingController);
    localStorage.clear();
  });

  afterEach(() => {
    http.verify();
    localStorage.clear();
  });

  it('register posts payload', () => {
    let id = '';
    svc.register({ email: 'z@y.com', password: 'P1!', firstName: 'N', lastName: 'S', role: 'User' } as any)
      .subscribe((r: any) => (id = r?.id ?? 'ok'));
    const req = http.expectOne(`${base}/register`);
    expect(req.request.method).toBe('POST');
    req.flush({ id: 'u1' });
    expect(id).toBe('u1');
  });

  it('logout clears state', () => {
    localStorage.setItem('token', 't');
    localStorage.setItem('roles', JSON.stringify(['User']));
    svc.logout();
    expect(svc.isLoggedIn()).toBeFalse();
    expect(svc.hasRole('User')).toBeFalse();
  });

  it('hasRole false when no token', () => {
    expect(svc.hasRole('User')).toBeFalse();
  });
});
