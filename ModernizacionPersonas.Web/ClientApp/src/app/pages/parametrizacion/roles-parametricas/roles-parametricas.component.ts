import { Component, OnInit, ViewChild } from "@angular/core";
import { MatDialog, MatDialogConfig } from "@angular/material/dialog";
import { MatPaginator } from "@angular/material/paginator";
import { MatSort } from "@angular/material/sort";
import { MatTableDataSource } from "@angular/material/table";
import {PageToolbarConfig,PageToolbarItem,} from "src/app/models/page-toolbar-item";
import { RolesUsuarioReaderService } from "src/app/services/roles-usuario.reader.service";
import { Router } from "@angular/router";
import { PageToolbarBuilder } from "src/app/shared/services/page-toolbar-builder";
import { Rol } from "src/app/models/rol";
import { RolEditarComponent } from "./components/rol-editar/rol-editar.component";
import { RolesWriterService } from "src/app/services/roles-writer.service";
import { RolCrearComponent } from "./components/rol-crear/rol-crear.component";

@Component({
  selector: "app-roles-parametricas",
  templateUrl: "./roles-parametricas.component.html",
  styleUrls: ["./roles-parametricas.component.scss"],
})
export class RolesParametricasComponent implements OnInit {
  isLoading: boolean = true;
  data: Rol[];
  dataSource: MatTableDataSource<Rol>;
  displayedColumns: string[] = ["nombre", "descripcion", "usuarioModifica", "fechaModifica", "opciones"];
  itemsCount: number;
  toolbarConfig: PageToolbarConfig;

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  constructor(
    private rolesReaderService: RolesUsuarioReaderService,
    private rolesWriterService: RolesWriterService,
    private dialog: MatDialog,
    private router: Router,
    private toolbarBuilder: PageToolbarBuilder
  ) {}

  ngOnInit() {
    this.loadRoles();
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
      }
      /*{
        name: "add",
        icon_path: "add",
        label: "Nuevo Rol",
        tooltip: "Nuevo Rol",
        isEnabled: true,
        fixed: true,
        onClick: () => {
          this.createRol();
        },
      },*/
    ];
    this.toolbarConfig = this.toolbarBuilder.build(items);
  }

  private loadRoles() {
    this.rolesReaderService.getRolesUsuario().subscribe((response) => {
      this.data = response;
      this.dataSource = new MatTableDataSource(this.data);
      this.itemsCount = this.dataSource.data.length;
      this.isLoading = false;
    });
  }

  refresh() {
    this.loadRoles();
    this.toolbarConfig.reset();
  }

  private editRol(rol: Rol) {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.direction = "ltr";
    dialogConfig.height = "auto";
    dialogConfig.width = "500px";
    
    dialogConfig.data = {
      title: "Editar Rol",
      codigo: rol.codigo,
      nombre: rol.nombre,
      descripcion: rol.descripcion,
      permisos: rol.permisos,
    };
    

    const dialogRef = this.dialog.open(RolEditarComponent, dialogConfig);

    dialogRef
      .afterClosed()
      .subscribe((data) =>
        this.rolesWriterService.editRol(data).subscribe((response) => this.refresh())
      );
  }

  private createRol() {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.direction = "ltr";
    dialogConfig.height = "auto";
    dialogConfig.width = "500px";

    dialogConfig.data = {
      title: "Crear Nuevo Rol",
    };

    const dialogRef = this.dialog.open(RolCrearComponent, dialogConfig);

    dialogRef
      .afterClosed()
      .subscribe((data) =>
        this.rolesWriterService
          .createRol(data)
          .subscribe((response) => this.refresh())
      );
  }

  applyFilter(filterValue: string) {
    this.dataSource.filter = filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }

    this.itemsCount = this.dataSource.data.length;
  }
}
