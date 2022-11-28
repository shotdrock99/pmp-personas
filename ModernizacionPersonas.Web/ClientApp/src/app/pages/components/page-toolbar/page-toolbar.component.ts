import { Component, OnInit, Input } from '@angular/core';
import { PageToolbarItem } from 'src/app/models/page-toolbar-item';

@Component({
  selector: 'app-page-toolbar',
  templateUrl: './page-toolbar.component.html',
  styleUrls: ['./page-toolbar.component.scss']
})
export class PageToolbarComponent implements OnInit {

  constructor() { }

  @Input('data') menuItems: PageToolbarItem[]

  ngOnInit() {
  }

  itemClick(event: Event, item: PageToolbarItem) {
    ;
    item.onClick(event);
  }
}
