//----------------------
// <auto-generated>
//     Generated using the NSwag toolchain v13.16.1.0 (NJsonSchema v10.7.2.0 (Newtonsoft.Json v12.0.0.0)) (http://NSwag.org)
// </auto-generated>
//----------------------

/* tslint:disable */
/* eslint-disable */
// ReSharper disable InconsistentNaming

import {
  mergeMap as _observableMergeMap,
  catchError as _observableCatch,
} from 'rxjs/operators';
import {
  Observable,
  throwError as _observableThrow,
  of as _observableOf,
} from 'rxjs';
import { Injectable, Inject, Optional, InjectionToken } from '@angular/core';
import {
  HttpClient,
  HttpHeaders,
  HttpResponse,
  HttpResponseBase,
} from '@angular/common/http';

export const API_BASE_URL = new InjectionToken<string>('API_BASE_URL');

@Injectable({
  providedIn: 'root',
})
export class CdeServerCallbackClient {
  private http: HttpClient;
  private baseUrl: string;
  protected jsonParseReviver: ((key: string, value: any) => any) | undefined =
    undefined;

  constructor(
    @Inject(HttpClient) http: HttpClient,
    @Optional() @Inject(API_BASE_URL) baseUrl?: string
  ) {
    this.http = http;
    this.baseUrl = baseUrl !== undefined && baseUrl !== null ? baseUrl : '';
  }

  handleCdeUploadCallback(
    state: string | null | undefined
  ): Observable<string> {
    let url_ = this.baseUrl + '/cde-server-callback/upload?';
    if (state !== undefined && state !== null)
      url_ += 'state=' + encodeURIComponent('' + state) + '&';
    url_ = url_.replace(/[?&]$/, '');

    let options_: any = {
      observe: 'response',
      responseType: 'blob',
      headers: new HttpHeaders({
        Accept: 'application/json',
      }),
    };

    return this.http
      .request('get', url_, options_)
      .pipe(
        _observableMergeMap((response_: any) => {
          return this.processHandleCdeUploadCallback(response_);
        })
      )
      .pipe(
        _observableCatch((response_: any) => {
          if (response_ instanceof HttpResponseBase) {
            try {
              return this.processHandleCdeUploadCallback(response_ as any);
            } catch (e) {
              return _observableThrow(e) as any as Observable<string>;
            }
          } else
            return _observableThrow(response_) as any as Observable<string>;
        })
      );
  }

  protected processHandleCdeUploadCallback(
    response: HttpResponseBase
  ): Observable<string> {
    const status = response.status;
    const responseBlob =
      response instanceof HttpResponse
        ? response.body
        : (response as any).error instanceof Blob
        ? (response as any).error
        : undefined;

    let _headers: any = {};
    if (response.headers) {
      for (let key of response.headers.keys()) {
        _headers[key] = response.headers.get(key);
      }
    }
    if (status === 200) {
      return blobToText(responseBlob).pipe(
        _observableMergeMap((_responseText) => {
          let result200: any = null;
          result200 =
            _responseText === ''
              ? null
              : (JSON.parse(_responseText, this.jsonParseReviver) as string);
          return _observableOf(result200);
        })
      );
    } else if (status !== 200 && status !== 204) {
      return blobToText(responseBlob).pipe(
        _observableMergeMap((_responseText) => {
          return throwException(
            'An unexpected server error occurred.',
            status,
            _responseText,
            _headers
          );
        })
      );
    }
    return _observableOf<string>(null as any);
  }

  handleCdeDownloadCallback(
    state: string | null | undefined,
    selectedDocumentsUrl: string | null | undefined,
    selectedUrl: string | null | undefined
  ): Observable<string> {
    let url_ = this.baseUrl + '/cde-server-callback/download?';
    if (state !== undefined && state !== null)
      url_ += 'state=' + encodeURIComponent('' + state) + '&';
    if (selectedDocumentsUrl !== undefined && selectedDocumentsUrl !== null)
      url_ +=
        'selected_documents_url=' +
        encodeURIComponent('' + selectedDocumentsUrl) +
        '&';
    if (selectedUrl !== undefined && selectedUrl !== null)
      url_ += 'selected_url=' + encodeURIComponent('' + selectedUrl) + '&';
    url_ = url_.replace(/[?&]$/, '');

    let options_: any = {
      observe: 'response',
      responseType: 'blob',
      headers: new HttpHeaders({
        Accept: 'application/json',
      }),
    };

    return this.http
      .request('get', url_, options_)
      .pipe(
        _observableMergeMap((response_: any) => {
          return this.processHandleCdeDownloadCallback(response_);
        })
      )
      .pipe(
        _observableCatch((response_: any) => {
          if (response_ instanceof HttpResponseBase) {
            try {
              return this.processHandleCdeDownloadCallback(response_ as any);
            } catch (e) {
              return _observableThrow(e) as any as Observable<string>;
            }
          } else
            return _observableThrow(response_) as any as Observable<string>;
        })
      );
  }

  protected processHandleCdeDownloadCallback(
    response: HttpResponseBase
  ): Observable<string> {
    const status = response.status;
    const responseBlob =
      response instanceof HttpResponse
        ? response.body
        : (response as any).error instanceof Blob
        ? (response as any).error
        : undefined;

    let _headers: any = {};
    if (response.headers) {
      for (let key of response.headers.keys()) {
        _headers[key] = response.headers.get(key);
      }
    }
    if (status === 200) {
      return blobToText(responseBlob).pipe(
        _observableMergeMap((_responseText) => {
          let result200: any = null;
          result200 =
            _responseText === ''
              ? null
              : (JSON.parse(_responseText, this.jsonParseReviver) as string);
          return _observableOf(result200);
        })
      );
    } else if (status !== 200 && status !== 204) {
      return blobToText(responseBlob).pipe(
        _observableMergeMap((_responseText) => {
          return throwException(
            'An unexpected server error occurred.',
            status,
            _responseText,
            _headers
          );
        })
      );
    }
    return _observableOf<string>(null as any);
  }
}

@Injectable({
  providedIn: 'root',
})
export class ClientProxyClient {
  private http: HttpClient;
  private baseUrl: string;
  protected jsonParseReviver: ((key: string, value: any) => any) | undefined =
    undefined;

  constructor(
    @Inject(HttpClient) http: HttpClient,
    @Optional() @Inject(API_BASE_URL) baseUrl?: string
  ) {
    this.http = http;
    this.baseUrl = baseUrl !== undefined && baseUrl !== null ? baseUrl : '';
  }

  getResponseViaBackend(
    targetUrl: string | null | undefined,
    accessToken: string | null | undefined
  ): Observable<FileResponse> {
    let url_ = this.baseUrl + '/client-proxy?';
    if (targetUrl !== undefined && targetUrl !== null)
      url_ += 'targetUrl=' + encodeURIComponent('' + targetUrl) + '&';
    if (accessToken !== undefined && accessToken !== null)
      url_ += 'accessToken=' + encodeURIComponent('' + accessToken) + '&';
    url_ = url_.replace(/[?&]$/, '');

    let options_: any = {
      observe: 'response',
      responseType: 'blob',
      headers: new HttpHeaders({
        Accept: 'application/octet-stream',
      }),
    };

    return this.http
      .request('get', url_, options_)
      .pipe(
        _observableMergeMap((response_: any) => {
          return this.processGetResponseViaBackend(response_);
        })
      )
      .pipe(
        _observableCatch((response_: any) => {
          if (response_ instanceof HttpResponseBase) {
            try {
              return this.processGetResponseViaBackend(response_ as any);
            } catch (e) {
              return _observableThrow(e) as any as Observable<FileResponse>;
            }
          } else
            return _observableThrow(
              response_
            ) as any as Observable<FileResponse>;
        })
      );
  }

  protected processGetResponseViaBackend(
    response: HttpResponseBase
  ): Observable<FileResponse> {
    const status = response.status;
    const responseBlob =
      response instanceof HttpResponse
        ? response.body
        : (response as any).error instanceof Blob
        ? (response as any).error
        : undefined;

    let _headers: any = {};
    if (response.headers) {
      for (let key of response.headers.keys()) {
        _headers[key] = response.headers.get(key);
      }
    }
    if (status === 200 || status === 206) {
      const contentDisposition = response.headers
        ? response.headers.get('content-disposition')
        : undefined;
      const fileNameMatch = contentDisposition
        ? /filename="?([^"]*?)"?(;|$)/g.exec(contentDisposition)
        : undefined;
      const fileName =
        fileNameMatch && fileNameMatch.length > 1
          ? fileNameMatch[1]
          : undefined;
      return _observableOf({
        fileName: fileName,
        data: responseBlob as any,
        status: status,
        headers: _headers,
      });
    } else if (status !== 200 && status !== 204) {
      return blobToText(responseBlob).pipe(
        _observableMergeMap((_responseText) => {
          return throwException(
            'An unexpected server error occurred.',
            status,
            _responseText,
            _headers
          );
        })
      );
    }
    return _observableOf<FileResponse>(null as any);
  }
}

@Injectable({
  providedIn: 'root',
})
export class DocumentsUploadHandlerClient {
  private http: HttpClient;
  private baseUrl: string;
  protected jsonParseReviver: ((key: string, value: any) => any) | undefined =
    undefined;

  constructor(
    @Inject(HttpClient) http: HttpClient,
    @Optional() @Inject(API_BASE_URL) baseUrl?: string
  ) {
    this.http = http;
    this.baseUrl = baseUrl !== undefined && baseUrl !== null ? baseUrl : '';
  }

  prepareDocumentUploadAndOpenSystemBrowser(
    parameters: DocumentUploadInitializationParameters
  ): Observable<void> {
    let url_ = this.baseUrl + '/documents-upload-handler/start-upload';
    url_ = url_.replace(/[?&]$/, '');

    const content_ = JSON.stringify(parameters);

    let options_: any = {
      body: content_,
      observe: 'response',
      responseType: 'blob',
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
      }),
    };

    return this.http
      .request('post', url_, options_)
      .pipe(
        _observableMergeMap((response_: any) => {
          return this.processPrepareDocumentUploadAndOpenSystemBrowser(
            response_
          );
        })
      )
      .pipe(
        _observableCatch((response_: any) => {
          if (response_ instanceof HttpResponseBase) {
            try {
              return this.processPrepareDocumentUploadAndOpenSystemBrowser(
                response_ as any
              );
            } catch (e) {
              return _observableThrow(e) as any as Observable<void>;
            }
          } else return _observableThrow(response_) as any as Observable<void>;
        })
      );
  }

  protected processPrepareDocumentUploadAndOpenSystemBrowser(
    response: HttpResponseBase
  ): Observable<void> {
    const status = response.status;
    const responseBlob =
      response instanceof HttpResponse
        ? response.body
        : (response as any).error instanceof Blob
        ? (response as any).error
        : undefined;

    let _headers: any = {};
    if (response.headers) {
      for (let key of response.headers.keys()) {
        _headers[key] = response.headers.get(key);
      }
    }
    if (status === 400) {
      return blobToText(responseBlob).pipe(
        _observableMergeMap((_responseText) => {
          return throwException(
            'A server side error occurred.',
            status,
            _responseText,
            _headers
          );
        })
      );
    } else if (status === 204) {
      return blobToText(responseBlob).pipe(
        _observableMergeMap((_responseText) => {
          return _observableOf<void>(null as any);
        })
      );
    } else if (status !== 200 && status !== 204) {
      return blobToText(responseBlob).pipe(
        _observableMergeMap((_responseText) => {
          return throwException(
            'An unexpected server error occurred.',
            status,
            _responseText,
            _headers
          );
        })
      );
    }
    return _observableOf<void>(null as any);
  }
}

@Injectable({
  providedIn: 'root',
})
export class DocumentsSelectionHandlerClient {
  private http: HttpClient;
  private baseUrl: string;
  protected jsonParseReviver: ((key: string, value: any) => any) | undefined =
    undefined;

  constructor(
    @Inject(HttpClient) http: HttpClient,
    @Optional() @Inject(API_BASE_URL) baseUrl?: string
  ) {
    this.http = http;
    this.baseUrl = baseUrl !== undefined && baseUrl !== null ? baseUrl : '';
  }

  prepareDocumentSelectionAndOpenSystemBrowser(
    parameters: DocumentSelectionInitializationParameters
  ): Observable<void> {
    let url_ = this.baseUrl + '/documents-selection-handler/start-selection';
    url_ = url_.replace(/[?&]$/, '');

    const content_ = JSON.stringify(parameters);

    let options_: any = {
      body: content_,
      observe: 'response',
      responseType: 'blob',
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
      }),
    };

    return this.http
      .request('post', url_, options_)
      .pipe(
        _observableMergeMap((response_: any) => {
          return this.processPrepareDocumentSelectionAndOpenSystemBrowser(
            response_
          );
        })
      )
      .pipe(
        _observableCatch((response_: any) => {
          if (response_ instanceof HttpResponseBase) {
            try {
              return this.processPrepareDocumentSelectionAndOpenSystemBrowser(
                response_ as any
              );
            } catch (e) {
              return _observableThrow(e) as any as Observable<void>;
            }
          } else return _observableThrow(response_) as any as Observable<void>;
        })
      );
  }

  protected processPrepareDocumentSelectionAndOpenSystemBrowser(
    response: HttpResponseBase
  ): Observable<void> {
    const status = response.status;
    const responseBlob =
      response instanceof HttpResponse
        ? response.body
        : (response as any).error instanceof Blob
        ? (response as any).error
        : undefined;

    let _headers: any = {};
    if (response.headers) {
      for (let key of response.headers.keys()) {
        _headers[key] = response.headers.get(key);
      }
    }
    if (status === 400) {
      return blobToText(responseBlob).pipe(
        _observableMergeMap((_responseText) => {
          return throwException(
            'A server side error occurred.',
            status,
            _responseText,
            _headers
          );
        })
      );
    } else if (status === 204) {
      return blobToText(responseBlob).pipe(
        _observableMergeMap((_responseText) => {
          return _observableOf<void>(null as any);
        })
      );
    } else if (status !== 200 && status !== 204) {
      return blobToText(responseBlob).pipe(
        _observableMergeMap((_responseText) => {
          return throwException(
            'An unexpected server error occurred.',
            status,
            _responseText,
            _headers
          );
        })
      );
    }
    return _observableOf<void>(null as any);
  }

  getDocumentSelectionCallbackUrl(
    clientState: string | null | undefined
  ): Observable<DocumentSelectionCallbackParameters> {
    let url_ = this.baseUrl + '/documents-selection-handler?';
    if (clientState !== undefined && clientState !== null)
      url_ += 'clientState=' + encodeURIComponent('' + clientState) + '&';
    url_ = url_.replace(/[?&]$/, '');

    let options_: any = {
      observe: 'response',
      responseType: 'blob',
      headers: new HttpHeaders({
        Accept: 'application/json',
      }),
    };

    return this.http
      .request('get', url_, options_)
      .pipe(
        _observableMergeMap((response_: any) => {
          return this.processGetDocumentSelectionCallbackUrl(response_);
        })
      )
      .pipe(
        _observableCatch((response_: any) => {
          if (response_ instanceof HttpResponseBase) {
            try {
              return this.processGetDocumentSelectionCallbackUrl(
                response_ as any
              );
            } catch (e) {
              return _observableThrow(
                e
              ) as any as Observable<DocumentSelectionCallbackParameters>;
            }
          } else
            return _observableThrow(
              response_
            ) as any as Observable<DocumentSelectionCallbackParameters>;
        })
      );
  }

  protected processGetDocumentSelectionCallbackUrl(
    response: HttpResponseBase
  ): Observable<DocumentSelectionCallbackParameters> {
    const status = response.status;
    const responseBlob =
      response instanceof HttpResponse
        ? response.body
        : (response as any).error instanceof Blob
        ? (response as any).error
        : undefined;

    let _headers: any = {};
    if (response.headers) {
      for (let key of response.headers.keys()) {
        _headers[key] = response.headers.get(key);
      }
    }
    if (status === 200) {
      return blobToText(responseBlob).pipe(
        _observableMergeMap((_responseText) => {
          let result200: any = null;
          result200 =
            _responseText === ''
              ? null
              : (JSON.parse(
                  _responseText,
                  this.jsonParseReviver
                ) as DocumentSelectionCallbackParameters);
          return _observableOf(result200);
        })
      );
    } else if (status !== 200 && status !== 204) {
      return blobToText(responseBlob).pipe(
        _observableMergeMap((_responseText) => {
          return throwException(
            'An unexpected server error occurred.',
            status,
            _responseText,
            _headers
          );
        })
      );
    }
    return _observableOf<DocumentSelectionCallbackParameters>(null as any);
  }

  openCdeDocumentSelectionPage(
    url: string | null | undefined
  ): Observable<FileResponse> {
    let url_ = this.baseUrl + '/documents-selection-handler?';
    if (url !== undefined && url !== null)
      url_ += 'Url=' + encodeURIComponent('' + url) + '&';
    url_ = url_.replace(/[?&]$/, '');

    let options_: any = {
      observe: 'response',
      responseType: 'blob',
      headers: new HttpHeaders({
        Accept: 'application/octet-stream',
      }),
    };

    return this.http
      .request('post', url_, options_)
      .pipe(
        _observableMergeMap((response_: any) => {
          return this.processOpenCdeDocumentSelectionPage(response_);
        })
      )
      .pipe(
        _observableCatch((response_: any) => {
          if (response_ instanceof HttpResponseBase) {
            try {
              return this.processOpenCdeDocumentSelectionPage(response_ as any);
            } catch (e) {
              return _observableThrow(e) as any as Observable<FileResponse>;
            }
          } else
            return _observableThrow(
              response_
            ) as any as Observable<FileResponse>;
        })
      );
  }

  protected processOpenCdeDocumentSelectionPage(
    response: HttpResponseBase
  ): Observable<FileResponse> {
    const status = response.status;
    const responseBlob =
      response instanceof HttpResponse
        ? response.body
        : (response as any).error instanceof Blob
        ? (response as any).error
        : undefined;

    let _headers: any = {};
    if (response.headers) {
      for (let key of response.headers.keys()) {
        _headers[key] = response.headers.get(key);
      }
    }
    if (status === 200 || status === 206) {
      const contentDisposition = response.headers
        ? response.headers.get('content-disposition')
        : undefined;
      const fileNameMatch = contentDisposition
        ? /filename="?([^"]*?)"?(;|$)/g.exec(contentDisposition)
        : undefined;
      const fileName =
        fileNameMatch && fileNameMatch.length > 1
          ? fileNameMatch[1]
          : undefined;
      return _observableOf({
        fileName: fileName,
        data: responseBlob as any,
        status: status,
        headers: _headers,
      });
    } else if (status !== 200 && status !== 204) {
      return blobToText(responseBlob).pipe(
        _observableMergeMap((_responseText) => {
          return throwException(
            'An unexpected server error occurred.',
            status,
            _responseText,
            _headers
          );
        })
      );
    }
    return _observableOf<FileResponse>(null as any);
  }
}

@Injectable({
  providedIn: 'root',
})
export class FileDownloadClient {
  private http: HttpClient;
  private baseUrl: string;
  protected jsonParseReviver: ((key: string, value: any) => any) | undefined =
    undefined;

  constructor(
    @Inject(HttpClient) http: HttpClient,
    @Optional() @Inject(API_BASE_URL) baseUrl?: string
  ) {
    this.http = http;
    this.baseUrl = baseUrl !== undefined && baseUrl !== null ? baseUrl : '';
  }

  downloadFile(
    downloadUrl: string | null | undefined
  ): Observable<FileResponse> {
    let url_ = this.baseUrl + '/file-download?';
    if (downloadUrl !== undefined && downloadUrl !== null)
      url_ += 'downloadUrl=' + encodeURIComponent('' + downloadUrl) + '&';
    url_ = url_.replace(/[?&]$/, '');

    let options_: any = {
      observe: 'response',
      responseType: 'blob',
      headers: new HttpHeaders({
        Accept: 'application/octet-stream',
      }),
    };

    return this.http
      .request('get', url_, options_)
      .pipe(
        _observableMergeMap((response_: any) => {
          return this.processDownloadFile(response_);
        })
      )
      .pipe(
        _observableCatch((response_: any) => {
          if (response_ instanceof HttpResponseBase) {
            try {
              return this.processDownloadFile(response_ as any);
            } catch (e) {
              return _observableThrow(e) as any as Observable<FileResponse>;
            }
          } else
            return _observableThrow(
              response_
            ) as any as Observable<FileResponse>;
        })
      );
  }

  protected processDownloadFile(
    response: HttpResponseBase
  ): Observable<FileResponse> {
    const status = response.status;
    const responseBlob =
      response instanceof HttpResponse
        ? response.body
        : (response as any).error instanceof Blob
        ? (response as any).error
        : undefined;

    let _headers: any = {};
    if (response.headers) {
      for (let key of response.headers.keys()) {
        _headers[key] = response.headers.get(key);
      }
    }
    if (status === 200 || status === 206) {
      const contentDisposition = response.headers
        ? response.headers.get('content-disposition')
        : undefined;
      const fileNameMatch = contentDisposition
        ? /filename="?([^"]*?)"?(;|$)/g.exec(contentDisposition)
        : undefined;
      const fileName =
        fileNameMatch && fileNameMatch.length > 1
          ? fileNameMatch[1]
          : undefined;
      return _observableOf({
        fileName: fileName,
        data: responseBlob as any,
        status: status,
        headers: _headers,
      });
    } else if (status === 400) {
      return blobToText(responseBlob).pipe(
        _observableMergeMap((_responseText) => {
          let result400: any = null;
          result400 =
            _responseText === ''
              ? null
              : (JSON.parse(_responseText, this.jsonParseReviver) as ApiError);
          return throwException(
            'A server side error occurred.',
            status,
            _responseText,
            _headers,
            result400
          );
        })
      );
    } else if (status !== 200 && status !== 204) {
      return blobToText(responseBlob).pipe(
        _observableMergeMap((_responseText) => {
          return throwException(
            'An unexpected server error occurred.',
            status,
            _responseText,
            _headers
          );
        })
      );
    }
    return _observableOf<FileResponse>(null as any);
  }
}

@Injectable({
  providedIn: 'root',
})
export class OpenIdCallbackClient {
  private http: HttpClient;
  private baseUrl: string;
  protected jsonParseReviver: ((key: string, value: any) => any) | undefined =
    undefined;

  constructor(
    @Inject(HttpClient) http: HttpClient,
    @Optional() @Inject(API_BASE_URL) baseUrl?: string
  ) {
    this.http = http;
    this.baseUrl = baseUrl !== undefined && baseUrl !== null ? baseUrl : '';
  }

  processOpenIdConnectCallback(
    serverResponse: string | null | undefined,
    code: string | null | undefined,
    error: string | null | undefined,
    state: string | null | undefined
  ): Observable<void> {
    let url_ = this.baseUrl + '/openid-connect-callback?';
    if (serverResponse !== undefined && serverResponse !== null)
      url_ += 'serverResponse=' + encodeURIComponent('' + serverResponse) + '&';
    if (code !== undefined && code !== null)
      url_ += 'code=' + encodeURIComponent('' + code) + '&';
    if (error !== undefined && error !== null)
      url_ += 'error=' + encodeURIComponent('' + error) + '&';
    if (state !== undefined && state !== null)
      url_ += 'state=' + encodeURIComponent('' + state) + '&';
    url_ = url_.replace(/[?&]$/, '');

    let options_: any = {
      observe: 'response',
      responseType: 'blob',
      headers: new HttpHeaders({}),
    };

    return this.http
      .request('get', url_, options_)
      .pipe(
        _observableMergeMap((response_: any) => {
          return this.processProcessOpenIdConnectCallback(response_);
        })
      )
      .pipe(
        _observableCatch((response_: any) => {
          if (response_ instanceof HttpResponseBase) {
            try {
              return this.processProcessOpenIdConnectCallback(response_ as any);
            } catch (e) {
              return _observableThrow(e) as any as Observable<void>;
            }
          } else return _observableThrow(response_) as any as Observable<void>;
        })
      );
  }

  protected processProcessOpenIdConnectCallback(
    response: HttpResponseBase
  ): Observable<void> {
    const status = response.status;
    const responseBlob =
      response instanceof HttpResponse
        ? response.body
        : (response as any).error instanceof Blob
        ? (response as any).error
        : undefined;

    let _headers: any = {};
    if (response.headers) {
      for (let key of response.headers.keys()) {
        _headers[key] = response.headers.get(key);
      }
    }
    if (status === 200) {
      return blobToText(responseBlob).pipe(
        _observableMergeMap((_responseText) => {
          return _observableOf<void>(null as any);
        })
      );
    } else if (status !== 200 && status !== 204) {
      return blobToText(responseBlob).pipe(
        _observableMergeMap((_responseText) => {
          return throwException(
            'An unexpected server error occurred.',
            status,
            _responseText,
            _headers
          );
        })
      );
    }
    return _observableOf<void>(null as any);
  }
}

@Injectable({
  providedIn: 'root',
})
export class OpenIdClient {
  private http: HttpClient;
  private baseUrl: string;
  protected jsonParseReviver: ((key: string, value: any) => any) | undefined =
    undefined;

  constructor(
    @Inject(HttpClient) http: HttpClient,
    @Optional() @Inject(API_BASE_URL) baseUrl?: string
  ) {
    this.http = http;
    this.baseUrl = baseUrl !== undefined && baseUrl !== null ? baseUrl : '';
  }

  initiateOpenIdConnectImplicitLogin(
    authenticationParameters: OpenIdConnectAuthenticationParameters
  ): Observable<void> {
    let url_ = this.baseUrl + '/openid-connect';
    url_ = url_.replace(/[?&]$/, '');

    const content_ = JSON.stringify(authenticationParameters);

    let options_: any = {
      body: content_,
      observe: 'response',
      responseType: 'blob',
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
      }),
    };

    return this.http
      .request('post', url_, options_)
      .pipe(
        _observableMergeMap((response_: any) => {
          return this.processInitiateOpenIdConnectImplicitLogin(response_);
        })
      )
      .pipe(
        _observableCatch((response_: any) => {
          if (response_ instanceof HttpResponseBase) {
            try {
              return this.processInitiateOpenIdConnectImplicitLogin(
                response_ as any
              );
            } catch (e) {
              return _observableThrow(e) as any as Observable<void>;
            }
          } else return _observableThrow(response_) as any as Observable<void>;
        })
      );
  }

  protected processInitiateOpenIdConnectImplicitLogin(
    response: HttpResponseBase
  ): Observable<void> {
    const status = response.status;
    const responseBlob =
      response instanceof HttpResponse
        ? response.body
        : (response as any).error instanceof Blob
        ? (response as any).error
        : undefined;

    let _headers: any = {};
    if (response.headers) {
      for (let key of response.headers.keys()) {
        _headers[key] = response.headers.get(key);
      }
    }
    if (status === 204) {
      return blobToText(responseBlob).pipe(
        _observableMergeMap((_responseText) => {
          return _observableOf<void>(null as any);
        })
      );
    } else if (status !== 200 && status !== 204) {
      return blobToText(responseBlob).pipe(
        _observableMergeMap((_responseText) => {
          return throwException(
            'An unexpected server error occurred.',
            status,
            _responseText,
            _headers
          );
        })
      );
    }
    return _observableOf<void>(null as any);
  }
}

export interface DocumentUploadInitializationParameters {
  accessToken: string;
  clientState: string;
  openCdeBaseUrl: string;
  files: DocumentUploadInitializationFile[];
}

export interface DocumentUploadInitializationFile {
  sessionFileId: string;
  fileName: string;
  fileSizeInBytes: number;
  filePath: string;
}

export interface DocumentSelectionInitializationParameters {
  accessToken: string;
  clientState: string;
  openCdeBaseUrl: string;
}

export interface DocumentSelectionCallbackParameters {
  callbackUrl: string;
}

/** Data transfer class to convey api errors */
export interface ApiError {
  /** This dictionary contains a set of all errors and their messages */
  errors?: { [key: string]: string[] } | undefined;
}

export interface OpenIdConnectAuthenticationParameters {
  flow: OpenIdConnectFlowType;
  clientState: string;
  clientConfiguration: OpenIdConnectClientConfiguration;
}

export enum OpenIdConnectFlowType {
  Implicit = 0,
  AuthorizationCode = 1,
}

export interface OpenIdConnectClientConfiguration {
  clientId: string;
  clientSecret?: string | undefined;
  authorizeEndpoint: string;
  tokenEndpoint?: string | undefined;
  requiredScope?: string | undefined;
  customRedirectUrl?: string | undefined;
}

export interface OpenIdConnectAuthenticationResult {
  isSuccess: boolean;
  jwtToken?: string | undefined;
  expiresAt?: number;
  clientState: string;
}

export interface SystemBrowserUrlOpenCommand {
  url?: string | undefined;
}

export interface FileResponse {
  data: Blob;
  status: number;
  fileName?: string;
  headers?: { [name: string]: any };
}

export class ApiException extends Error {
  message: string;
  status: number;
  response: string;
  headers: { [key: string]: any };
  result: any;

  constructor(
    message: string,
    status: number,
    response: string,
    headers: { [key: string]: any },
    result: any
  ) {
    super();

    this.message = message;
    this.status = status;
    this.response = response;
    this.headers = headers;
    this.result = result;
  }

  protected isApiException = true;

  static isApiException(obj: any): obj is ApiException {
    return obj.isApiException === true;
  }
}

function throwException(
  message: string,
  status: number,
  response: string,
  headers: { [key: string]: any },
  result?: any
): Observable<any> {
  if (result !== null && result !== undefined) return _observableThrow(result);
  else
    return _observableThrow(
      new ApiException(message, status, response, headers, null)
    );
}

function blobToText(blob: any): Observable<string> {
  return new Observable<string>((observer: any) => {
    if (!blob) {
      observer.next('');
      observer.complete();
    } else {
      let reader = new FileReader();
      reader.onload = (event) => {
        observer.next((event.target as any).result);
        observer.complete();
      };
      reader.readAsText(blob);
    }
  });
}
