<?php
header('x-powered-by: TYX/0301');
header('server: TYX_Server/0.233');

if (isset($_GET['key'])){
	if ($_GET['key'] == "_caesar_salad_"){
	    print "<p>_caesar_salad_</p>";
		print "The flag is: <code>flag{r0maine_lettuce}</code>";
	}
	else if ($_GET['key'] == "_fdhvdu_vdodg_"){
	    print "<p>不对哦,来试试看</p>";
		print "<p>abc -> def</p>";
	}
	else {
	    print "<p>错误的Flag</p>";
    }

}
?>

<html>
<head>
<link rel="icon" href="favicon.ico" type="image/x-icon" />
<title>著名的沙拉</title>
</head>
<body>
<h1>著名的沙拉</h1>
<p>沙拉的精髓在于食材的混合与调和。</p>
<p>但是加密算法就不一样了，多种古老的算法联用一样也没有用的，即便是你吃三份沙拉也没有用。</p>
<p>因为阅读任何一本现代密码学的基础书籍，你将都会被提醒：不要自己发明加密算法或协议（Please don’t try to invent your own encryption algorithm or protocol.）</p>

<form>
 <p>请尝试解密 <code>X2ZkaHZkdV92ZG9kZ18=</code> </p>
  <p>Key: <input type="text" name="key"/></p>
  <p><input type="submit" value="提交" /></p>
</form>
</body>
</html>
