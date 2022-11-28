import { Component, OnInit } from '@angular/core';
import { VariablesGlobalesReaderService } from 'src/app/services/variables-globales-reader.service';
import { PageToolbarItem, PageToolbarConfig } from 'src/app/models/page-toolbar-item';
import { PageToolbarBuilder } from 'src/app/shared/services/page-toolbar-builder';
import { Router } from '@angular/router';
import { VariablesGlobalesWriterService } from 'src/app/services/variables-globales-writer.service';
import { ParametrizacionApp } from 'src/app/models/parametrizacion-app';

@Component({
  selector: "app-variables-globales-parametricas",
  templateUrl: "./variables-globales-parametricas.component.html",
  styleUrls: ["./variables-globales-parametricas.component.scss"],
})
export class VariablesGlobalesParametricasComponent implements OnInit {

  isLoading: boolean = true;
  data: ParametrizacionApp[];
  toolbarConfig: PageToolbarConfig;

  constructor(
    private variablesGlobalesReaderService: VariablesGlobalesReaderService,
    private variablesGlobalesWriterService: VariablesGlobalesWriterService,
    private toolbarBuilder: PageToolbarBuilder,
    private router: Router
  ) {}

  ngOnInit() {
    this.loadVariablesGlobales();
    this.initializeToolbar();
  }

  initializeToolbar() {
    const items: PageToolbarItem[] = [
      {
        name: "home",
        icon_path: "home",
        label: "",
        tooltip: "Parametrización",
        isEnabled: true,
        fixed: true,
        onClick: () => {
          this.router.navigate(["/parametrizacion"]);
        },
      },
      {
        name: "refresh",
        icon_path: "refresh",
        label: "",
        tooltip: "Refrescar",
        isEnabled: true,
        fixed: true,
        onClick: () => {
          this.refresh();
        },
      },
    ];
    this.toolbarConfig = this.toolbarBuilder.build(items);
  }

  refresh() {
    this.loadVariablesGlobales();
    this.toolbarConfig.reset();
  }

  private loadVariablesGlobales() {
    this.variablesGlobalesReaderService
      .getVariablesGlobales()
      .subscribe((response) => {
        this.data = response;
        this.isLoading = false;
      });
  }

  private edit(variable: ParametrizacionApp) {
    this.variablesGlobalesWriterService
      .editVariablesGlobales(variable)
      .subscribe((response) => this.refresh());
  }
}
