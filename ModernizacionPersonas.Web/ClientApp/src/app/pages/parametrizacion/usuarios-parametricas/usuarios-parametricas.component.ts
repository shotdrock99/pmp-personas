import { UsersWriterService } from './../../../services/users-writer.service';
import { PageToolbarBuilder } from 'src/app/shared/services/page-toolbar-builder';
import { Router } from '@angular/router';
import { PageToolbarConfig, PageToolbarItem } from 'src/app/models/page-toolbar-item';
import { Component, OnInit, ViewChild } from '@angular/core';
import { UsuarioPersonas } from 'src/app/models/usuario-personas';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { UsersReaderService } from 'src/app/services/users-reader.service';
import { UsuarioEditarComponent } from './components/usuario-editar/usuario-editar.component';
import { UsuarioCrearComponent } from './components/usuario-crear/usuario-crear.component';
import { IntermediarioCrearComponent } from './components/intermediario-crear/intermediario-crear.component';

@Component({
  selector: "app-usuarios-parametricas",
  templateUrl: "./usuarios-parametricas.component.html",
  styleUrls: ["./usuarios-parametricas.component.scss"],
})
export class UsuariosParametricasComponent implements OnInit {
  isLoading: boolean = true;
  data: UsuarioPersonas[];
  dataSource: MatTableDataSource<UsuarioPersonas>;
  displayedColumns: string[] = [
    "userName",
    "userEmail",
    "userNick",
    "roles",
    "usuarioModifica",
    "fechaModifica",
    "opciones",
  ];
  itemsCount: number;
  toolbarConfig: PageToolbarConfig;

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  constructor(
    private usersReaderService: UsersReaderService,
    private usersWriterService: UsersWriterService,
    private dialog: MatDialog,
    private router: Router,
    private toolbarBuilder: PageToolbarBuilder
  ) {}

  ngOnInit() {
    this.loadUsers();
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
      {
        name: "add",
        icon_path: "add",
        label: "Nuevo Usuario",
        tooltip: "Nuevo Usuario",
        isEnabled: true,
        fixed: true,
        onClick: () => {
          this.createUsuario();
        },
      },
      {
        name: "add",
        icon_path: "add",
        label: "Nuevo Intermediario",
        tooltip: "Nuevo Intermediario",
        isEnabled: true,
        fixed: true,
        onClick: () => {
          this.createIntermediario();
        },
      }
    ];
    this.toolbarConfig = this.toolbarBuilder.build(items);
  }

  refresh() {
    this.loadUsers();
    this.toolbarConfig.reset();
  }

  private loadUsers() {
    this.usersReaderService.getUsers().subscribe((response) => {
      this.data = response;
      // Assign the data to the data source for the table to render
      this.dataSource = new MatTableDataSource(this.data);
      this.itemsCount = this.dataSource.data.length;
      this.isLoading = false;
    });
  }

  public editUsuario(usuario: UsuarioPersonas) {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.direction = "ltr";
    dialogConfig.height = "auto";
    dialogConfig.width = "400px";

    dialogConfig.data = {
      title: "Editar Usuario",
      name: usuario.name,
      email: usuario.email,
      userId: usuario.userId,
      userName: usuario.userName,
      rol: usuario.rol,
    };

    const dialogRef = this.dialog.open(UsuarioEditarComponent, dialogConfig);

    dialogRef
      .afterClosed()
      .subscribe((data) =>
        this.usersWriterService
          .updateUser(data)
          .subscribe((response) => this.refresh())
      );
  }

  public disableUsuario(usuario: UsuarioPersonas) {
    this.usersWriterService
      .disableUser(usuario.userId)
      .subscribe((response) => this.refresh());
  }

  public createUsuario() {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.direction = "ltr";
    dialogConfig.height = "auto";
    dialogConfig.width = "400px";

    dialogConfig.data = {
      title: "Crear Nuevo Usuario",
    };

    const dialogRef = this.dialog.open(UsuarioCrearComponent, dialogConfig);

    dialogRef
      .afterClosed()
      .subscribe((data) =>
        this.usersWriterService
          .createUser(data)
          .subscribe((response) => this.refresh())
      );
  }

  public createIntermediario() {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.direction = "ltr";
    dialogConfig.height = "auto";
    dialogConfig.width = "400px";

    dialogConfig.data = {
      title: "Crear Nuevo Intermediario",
    };

    const dialogRef = this.dialog.open(IntermediarioCrearComponent, dialogConfig);

    dialogRef
      .afterClosed()
      .subscribe((data) =>
        this.usersWriterService
          .createIntermediario(data)
          .subscribe((response) => response)
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
