import jsPDF from 'jspdf/dist/jspdf.debug.js';
import html2canvas from 'html2canvas';
import html2pdf from 'html2pdf.js';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class PrintService {
  constructor() {
    window['html2canvas'] = html2canvas;
  }

  options: PrintOptions;

  margins = {
    top: 80,
    posY: 60,
    left: 40,
    width: 522
  };

  printHtml2Pdf(element, options?: PrintOptions) {
    let headerImgUri = options.headerImageUri;

    var opt = {
      margin: 1,
      filename: 'myfile.pdf',
      image: { type: 'jpeg', quality: 0.98 },
      html2canvas: { scale: 1 },
      jsPDF: { unit: 'in', format: 'letter', orientation: 'portrait' }
    };

    // New Promise-based usage:
    html2pdf().set(opt).from(element).save();

    // Old monolithic-style usage:
    //html2pdf(element, opt);
  }

  printFromHtml(element, options?: PrintOptions) {
    var pdf = new jsPDF('p', 'pt', 'letter');
    // we support special element handlers. Register them with jQuery-style
    // ID selector for either ID or node name. ("#iAmID", "div", "span" etc.)
    // There is no support for any other type of selectors
    // (class, of compound) at this time.
    const specialElementHandlers = {
      // element with id of "bypass" - jQuery style selector
      '#bypassme': function (element, renderer) {
        // true = "handled elsewhere, bypass text extraction"
        return true
      }
    };
    // all coords and widths are in jsPDF instance's declared units
    // 'inches' in this case
    pdf.fromHTML(
      element, // HTML string or DOM elem ref.
      this.margins.left, // x coord
      this.margins.top, { // y coord
      'width': this.margins.width, // max width of content on PDF
      //'elementHandlers': specialElementHandlers
    }, function (dispose) {
      // dispose: object with X, Y of the last line add to the PDF
      //          this allow the insertion of new lines after html
      //pdf.save('Test.pdf');
      pdf.text('Pie de pagina', dispose.x, dispose.y);
      window.open(pdf.output('bloburl'));
    }, this.margins);
  }

  private getBlobUrl(element: any, options: any, canvas: any) {
    let w = 810;
    let h = 825;

    let pdf = new jsPDF('p', 'pt', 'letter');
    let pageCount = element.clientHeight / h;
    pageCount = Math.floor(pageCount);

    const img = new Image();
    img.src = options.headerImageUri;

    const img2 = new Image();
    img2.src = options.footerImageUri;

    for (let i = 0; i <= pageCount - 1; i++) {
      // This is all just html2canvas stuff
      let srcImg = canvas;
      // source area
      let sX = 0;
      let sY = (h - 5) * i; // start pixels down for every new page
      let sWidth = w;
      let sHeight = h;

      // destination area
      let dX = 0;
      let dY = 0;
      let dWidth = w;
      let dHeight = h;

      window['onePageCanvas'] = document.createElement("canvas");
      let onePageCanvas = window['onePageCanvas'];
      onePageCanvas.setAttribute('width', w);
      onePageCanvas.setAttribute('height', h);
      let ctx = onePageCanvas.getContext('2d');
      // details on this usage of this function:
      // https://developer.mozilla.org/en-US/docs/Web/API/Canvas_API/Tutorial/Using_images#Slicing
      ctx.drawImage(srcImg, sX, sY, sWidth, sHeight, dX, dY, dWidth, dHeight);

      // document.body.appendChild(canvas);
      let canvasDataURL = onePageCanvas.toDataURL("image/png", 1.0);

      let width = onePageCanvas.width;
      let height = onePageCanvas.clientHeight;

      // If we're on anything other than the first page,
      // add another page
      if (i > 0) {
        pdf.addPage(612, 791); //8.5" x 11" in pts (in*72)
      }
      // declare that we're working on that page
      pdf.setPage(i + 1);

      // add header image
      pdf.addImage(img, 'PNG', 0, 0, 612.0, 70.0);
      // add header image
      const posY = pdf.internal.pageSize.height - 70;
      pdf.addImage(img2, 'PNG', 0, posY, 612.0, 70.0);
      // add content to that page!
      pdf.addImage(canvasDataURL, 'PNG', 0, 80, (width * .76), (height * .76));

      // add count page info
      let page = pdf.internal.getCurrentPageInfo().pageNumber;
      pdf.setFontSize(10);
      const posX = pdf.internal.pageSize.width - 100;
      pdf.text(`Página ${page} de ${pageCount}`, posX, pdf.internal.pageSize.height - 10);
    }
    // after the for loop is finished running, we save the pdf.
    //pdf.save('Test.pdf');
    let url = pdf.output('bloburl');

    return url;
  }

  printFromImage(element, options?: PrintOptions, callback?: any) {
    html2canvas(element)
      .then(async (canvas) => {
        const blobUrl = this.getBlobUrl(element, options, canvas);
        window.open(blobUrl);
        if (callback) {
          callback();
        }
      });
  }

  getBlob(element, options?: PrintOptions, callback?: any): Promise<any> {
    return new Promise((resolve, reject) => {
      html2canvas(element)
        .then(async (canvas) => {
          const blobUrl = this.getBlobUrl(element, options, canvas);
          let blob = await fetch(blobUrl).then(r => r.blob());

          resolve(blob);
        });
    });
  }

  printHtml(element, options?: PrintOptions) {
    const content = element;

    // crea el documento
    let pdf = new jsPDF({
      unit: 'pt',
      format: [612, 792],//'letter',
      orientation: 'portrait'
    });

    let opts = {
      x: 40,
      y: 60,
      margin: [0, 0, 100, 0],
      pagesplit: true,
      html2canvas: {
        scale: 0.6, // default is window.devicePixelRatio
      },
      callback: function (res) {
        let pageCount = res.getNumberOfPages();
        for (let i = 0; i <= pageCount; i++) {
          // declare that we're working on that page
          pdf.setPage(i + 1);
          // add header image
          //pdf.addImage(headerImgUri, 'PNG', 0, 0);

          let page = pdf.internal.getCurrentPageInfo().pageNumber;
          pdf.setFontSize(10);
          pdf.text(`Página ${page} de ${pageCount}`, 10, pdf.internal.pageSize.height - 10);
        }

        window.open(pdf.output('bloburl'));
      }
    };

    pdf.html(content, opts);
  }

  toDataURL(url, callback) {
    var xhr = new XMLHttpRequest();
    xhr.onload = function () {
      var reader = new FileReader();
      reader.onloadend = function () {
        callback(reader.result);
      }
      reader.readAsDataURL(xhr.response);
    };
    xhr.open('GET', url);
    xhr.responseType = 'blob';
    xhr.send();
  }
}

export interface PrintOptions {
  headerImageUri: string;
  footerImageUri: string;
  codigoRamo: number;
  codigoSubramo: number;
}
