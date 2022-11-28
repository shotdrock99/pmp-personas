import { Injectable, Output, EventEmitter, Directive } from '@angular/core';
import { HttpClient, HttpEventType } from '@angular/common/http';
import { map } from 'rxjs/operators';

@Directive()
@Injectable({
  providedIn: 'root'
})
export class UploadFileService {

  @Output()
  public onUploadFinished = new EventEmitter();
  public onUploadFail = new EventEmitter();

  constructor(private httpClient: HttpClient) { }

  upload(uploadURL, data) {
    return this.httpClient.post<any>(uploadURL, data, {
      reportProgress: true,
      observe: 'events'
    }).pipe(map((event) => {
      switch (event.type) {
        case HttpEventType.UploadProgress:
          let progress = Math.round(100 * event.loaded / event.total);
          return { status: 'progress', message: progress };

        case HttpEventType.Response:
          return event.body;
        default:
          return `Unhandled event: ${event.type}`;
      }
    }));
  }

  upload2(uploadURL, data) {
    this.httpClient.post(uploadURL, data, {
      reportProgress: true,
      observe: 'events'
    }).subscribe(event => {
      if (event.type === HttpEventType.UploadProgress) {
        let progress = Math.round(100 * event.loaded / event.total);
      }
      else if (event.type === HttpEventType.Response) {
        this.onUploadFinished.emit(event.body);
      }
    }, err => {
      this.onUploadFail.emit(err);
    });
  }
}
