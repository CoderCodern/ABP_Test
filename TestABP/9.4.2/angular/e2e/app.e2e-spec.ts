import { TestABPTemplatePage } from './app.po';

describe('TestABP App', function() {
  let page: TestABPTemplatePage;

  beforeEach(() => {
    page = new TestABPTemplatePage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});
