/**
 * @license Copyright (c) 2003-2020, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see https://ckeditor.com/legal/ckeditor-oss-license
 */

CKEDITOR.editorConfig = function( config ) {
	// Define changes to default configuration here. For example:
	// config.language = 'fr';
	// config.uiColor = '#AADC6E';
	
	config.syntaxhighlight_lang = 'csharp';
	config.syntaxhighlight_hideControls = true;
	config.language = 'vi';
	config.filebrowserBrowserUrl = '/Assets/admin/js/plugins/ckfinder/ckfinder.html';
	config.filebrowserImageBrowserUrl = '/Assets/admin/js/plugins/ckfinder.html?Type=Image';
	config.filebrowserFlashBrowserUrl = '/Assets/admin/js/plugins/ckfinder.html?Type=Flash';
	config.filebrowserUpLoadUrl = '/Assets/admin/js/plugins/ckfinder/core/connector/aspx/connetor.aspx?command=QuikUpload&type=Files';

	config.filebrowserImageUpLoadUrl = '/Data';
	config.filebrowserFlashUpLoadUrl = '/Assets/admin/js/plugins/ckfinder/core/connector/aspx/connetor.aspx?command=QuikUpload&type=Flash';

	CKFinder.setupCKEditor(null, '/Assets/admin/js/plugins/ckfinder/');

	                                                                                           
};
