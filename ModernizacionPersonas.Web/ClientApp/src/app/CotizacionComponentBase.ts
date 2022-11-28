export class CotizacionComponentBase {
  displayFn(field, item): string {
    if (item) {
      return item[field];
    }
  }
}
