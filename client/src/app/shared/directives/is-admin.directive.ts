import { Directive, effect, inject, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { AccountService } from '../../core/services/account.service';

@Directive({
  selector: '[appIsAdmin]' // *appIsAdmin
})
export class IsAdminDirective {
  private accountService = inject(AccountService);
  private viewContainerRef = inject(ViewContainerRef);
  private templateReg = inject(TemplateRef);

  constructor() {
    effect(() => {
      if (this.accountService.isAdmin()) {
        this.viewContainerRef.createEmbeddedView(this.templateReg);
      }
      else {
        this.viewContainerRef.clear();
      }
    })
   }



}
