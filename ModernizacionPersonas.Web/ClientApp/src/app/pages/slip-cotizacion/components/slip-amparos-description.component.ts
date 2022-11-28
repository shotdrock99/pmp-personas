import { Component, OnInit, Input, ComponentRef, ComponentFactoryResolver, Compiler, ComponentFactory, NgModule, ModuleWithComponentFactories, ViewChild, ViewContainerRef } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-slip-amparos-description',
  template: '<div #container>{{template}}</div>'
})
export class SlipAmparosDescriptionComponent implements OnInit {
  private componentRef: ComponentRef<{}>;

  constructor(private componentFactoryResolver: ComponentFactoryResolver,
    private compiler: Compiler) { }

  @Input('template')
  template: string;

  @Input('model')
  model: any;

  @ViewChild('container', { read: ViewContainerRef, static: true })
  container: ViewContainerRef;

  ngOnInit() {
    //this.compileTemplate();
  }

  compileTemplate() {
    let metadata = {
      selector: 'dynamic-component',
      template: this.template
    };

    let factory = this.createComponentFactorySync(this.compiler, metadata, this.model.variables);
    if (this.componentRef) {
      this.componentRef.destroy();
      this.componentRef = null;
    }

    this.componentRef = this.container.createComponent(factory);
  }

  createComponentFactorySync(compiler: Compiler, metadata: Component, componentClass: any): ComponentFactory<any> {
    const cmpClass = componentClass || class RuntimeComponent { componentClass };
    const decoratedCmp = Component(metadata)(cmpClass);

    const moduleClass = class RuntimeComponentModule {
    };
    const decoratedNgModule = NgModule({ imports: [], declarations: [decoratedCmp] })(moduleClass);

    const module: ModuleWithComponentFactories<any> = compiler.compileModuleAndAllComponentsSync(decoratedNgModule);
    /*@NgModule({ imports: [CommonModule], declarations: [decoratedCmp] })
    class RuntimeComponentModule { }

    let module: ModuleWithComponentFactories<any> = compiler.compileModuleAndAllComponentsSync(RuntimeComponentModule);*/
    return module.componentFactories.find(f => f.componentType === decoratedCmp);
  }
}
