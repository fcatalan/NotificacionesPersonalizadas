import { NotificacionesPage } from './app.po';

describe('notificaciones App', function() {
  let page: NotificacionesPage;

  beforeEach(() => {
    page = new NotificacionesPage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});
