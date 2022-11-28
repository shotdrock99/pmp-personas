import { Injectable } from '@angular/core';
import { PageToolbarConfig } from 'src/app/models/page-toolbar-item';
import { PageToolbarItem } from './../../models/page-toolbar-item';

@Injectable({
  providedIn: 'root'
})
export class PageToolbarBuilder {

  items: PageToolbarItem[];

  constructor() { }

  private enableItems(items: PageToolbarItem[], itemNames: string[], value: boolean) {
    itemNames.forEach(itemName => {
      const item = items.find(x => x.name === itemName);
      if (item) {
        this.enableItem(items, itemName, value);
        if (item.items && item.items.length > 0) {
          this.enableItems(item.items, itemNames, value);
        }
      }
    });
  }

  private enableItem(items: PageToolbarItem[], itemName: string, value: boolean) {
    const item = items.find(x => x.name === itemName);
    if (item) {
      item.isEnabled = value;
    }
  }

  private reset(items: PageToolbarItem[]) {
    items.forEach(x => {
      if (!x.fixed) {
        x.isEnabled = false
      }

      if (x.items && x.items.length > 0) {
        this.reset(x.items);
      }
    });
  }

  build(items: PageToolbarItem[]): PageToolbarConfig {
    this.items = items;

    return {
      items: this.items,
      enableItem: (itemName: string, value: boolean) => {
        this.enableItem(this.items, itemName, value);
      },
      enableItems: (itemNames: string[], value: boolean) => {
        this.enableItems(this.items, itemNames, value);
      },
      reset: () => {
        this.reset(this.items);
      }
    };
  }
}
