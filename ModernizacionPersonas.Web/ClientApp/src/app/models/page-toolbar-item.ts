export class PageToolbarItem {
  public name?: string;
  public icon_path?: string;
  public label?: string;
  public tooltip?: string;
  public isEnabled?: boolean = false;
  public fixed?: boolean = false;
  public separator?: boolean = false;
  public items?: PageToolbarItem[];
  public onClick?: any;
}

export class PageToolbarConfig {
  public items: PageToolbarItem[];
  public enableItem: any;
  public enableItems: any;
  public reset: any;
}
