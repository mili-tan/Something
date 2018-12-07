<?php
header('x-powered-by: TYX_DSLang(NotPHP)');
header('server: TYX_Server/0.233');

function isHeicoreLatest() {
  if(isset($_SERVER['HTTP_USER_AGENT'])) {
    $userAgent = $_SERVER['HTTP_USER_AGENT'];
    $keyword = "HEICORE";
    if (stristr($userAgent, $keyword) !== false) {
      return true;
    }
  }
  return false;
}

if(isHeicoreLatest())
{
	print "<p>The flag is: <code>flag{Do_n0t_be_evi1}</code></p>";
}
else
{
	print "<p>为了文档安全，请使用最新版本黑芯浏览器（HEICORE）访问。</p>";
}

?>

<html>
<head>
<link rel="icon" href="favicon.ico" type="image/x-icon" />
<title>FLAG.txt</title>
</head>
<body>
<h1>FLAG.txt</h1>
</body>
</html>