var ckEditorCreated = false;

exports.mostrarAlerta = function(titulo, cuerpo) {
	$("#alertTitle").html(titulo);
	$("#alertContent").html(cuerpo);
	$("#alert").modal("show");
}

exports.inicializarCKEditor = function(idCajaTexto) {
		CKEDITOR.config.extraPlugins = "base64image,onchange";
		CKEDITOR.replace( idCajaTexto, {language: 'es'});
		CKEDITOR.config.toolbar = [
			{ name: 'clipboard', items : [ 'Cut','Copy','Paste','PasteText','PasteFromWord','-','Undo','Redo' ] },
			{ name: 'editing', items : [ 'Find','Replace','-','SelectAll','-','SpellChecker'] },
			{ name: 'basicstyles', items : [ 'Bold','Italic','Underline','Strike','Subscript','Superscript','-','RemoveFormat' ] },
			{ name: 'paragraph', items : [ 'NumberedList','BulletedList','-','Outdent','Indent','-','Blockquote',
			'-','JustifyLeft','JustifyCenter','JustifyRight','JustifyBlock'] },
			'/',
			{ name: 'links', items : [ 'Link','Unlink' ] },
			{ name: 'base64imageGroup', items: ['base64image']},
			{ name: 'insert', items : [ 'Table'] },
			{ name: 'styles', items : [ 'Styles','Format','Font','FontSize' ] },
			{ name: 'colors', items : [ 'TextColor','BGColor' ] }
		];
}

exports.destruirCKEditor = function() {
	for(name in CKEDITOR.instances)
	{
		CKEDITOR.instances[name].destroy(true);
	}	
}

exports.extraerHTMLCKEDITOR = function(idCajaTexto) {
	result = CKEDITOR.instances[idCajaTexto].getData();
	return result;
}

exports.onchangeCKEDITOR = function(idCajaTexto, funcion) {
	var result = "";
	CKEDITOR.instances[idCajaTexto].on("change", funcion)
}

exports.readyCKEDITOR = function(idCajaTexto, funcion) {
	CKEDITOR.instances[idCajaTexto].on("loaded", funcion);
}

exports.normalizarAlineacionImagenes = function(html) {
		var regexImg = /(<[iI][mM][gG]\s+[\sa-zA-Z0-9"';:=\-]*\s*[sS][rR][cC]\s*=\s*["']data:image\/[a-zA-Z0-9\/]+;base64,[a-zA-Z0-9\/\+=]+["'])([\sa-zA-Z0-9"';:=\-]+)?(\/?)(>)/g;
		var regexpFloatLeft = /[\s]*float[\s]*:[\s]*(left|right)/
		//var myRegexp = /<[iI][mM][gG]\s+/g;
		match = regexImg.exec(html);
		var matches = [];
		var replaces = [];
		var replaceStr;
		var i = 1;
		var matchFloat;
		var align;
		while (match != null) {
			align = "";
		  // matched text: match[0]
		  // match start: match.index
		  // capturing group n: match[n]
		  // console.log("Match "+i+"\n");
		  // console.log(match[0]+"\n")
		  // console.log(match[1]+"\n")
		  // console.log(match[2]+"\n")
		  // console.log(match[3]+"\n")
		  // console.log(match[4]+"\n")
		  matchFloat = regexpFloatLeft.exec(match[0]);
		  if (matchFloat != null && match[2].indexOf(" align:") == -1) {
			// console.log(matchFloat[0]);
			// console.log(matchFloat[1]);
			align = "align=\""+matchFloat[1]+"\"";
		  }
		  matches.push(match[0]);
		  replaceStr = match[1]+match[2]+" "+align+" "+match[3]+match[4];
		  replaces.push(replaceStr);
		  i++;
		  match = regexImg.exec(html);
		}
		//myRegexp = /(<[iI][mM][gG]\s+[\sa-zA-Z0-9"';:=\-]*\s*[sS][rR][cC]\s*=\s*["']data:image\/[a-zA-Z0-9\/]+;base64,[a-zA-Z0-9\/\+=]+["'])([\sa-zA-Z0-9"';:=\-]+)?(\/?)>/g;
		for (i=0;i<matches.length;i++) {
			html = html.replace(matches[i],replaces[i]);
		}
		return html;
}

exports.cambiarHTMLTextEditor = function(idCajaTexto, html) {
	CKEDITOR.instances[idCajaTexto].setData(html, function() { this.updateElement(); this.resetDirty(); console.log("Texto terminado de cambiar");});;
	//CKEDITOR.instances[i].setData(html, function() { this.updateElement(); console.log(this.getData());});
}

exports.IrAlInicioPagina = function() {
	$("html, body").animate({ scrollTop: 0 }, 300);
}