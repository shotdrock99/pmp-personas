
export class ErrorCollection {
  constructor() {
    Object.setPrototypeOf(this, ErrorCollection.prototype);
  }

  add(error: any) {
    let keys = Object.keys(error);
    let key = keys[0];
    let value = error[key];
    this[key] = value;
  }
}
