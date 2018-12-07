<?php
header('x-powered-by: TYX/0301');
header('server: TYX_Server/0.233');

if ($_SERVER['REQUEST_METHOD'] != "GET" || $_SERVER['REQUEST_METHOD'] != "GET") {
	print "flag{Nice2MEET_U}";
	return;
}

if (isset($_GET['key'])){
	if ($_GET['key'] == "Hello_World"){
	    print "<p>The flag is: <code>flag{Have_Fun}</code></p>";
		print "<p>接下来 再试试看其他方式请求这个页面~</p>";
	}
	else {
	    print "<p>错误的Flag</p>";
    }

}
?>

<html>
<head>
<link rel="icon" href="favicon.ico" type="image/x-icon" />
<title>签到题</title>
</head>
<body>
<h1>签到题</h1>
<p> 本题的主要作用是向你展示获取 flag 的一般步骤：</p>
<ol>
  <li>打开题目页面；（也就是本页面，你应该已经完成了）</li>
  <li>解题；（找到 flag）</li>
  <li>回到比赛平台提交 flag；</li>
  <li>完成！</li>
</ol>

<form>
 <p>是不是很简单！在本题中，你只要提交 <code>Hello_World</code> 就可以得到 flag</p>
  <p>Key: <input type="text" name="key" maxlength="5"/></p>
  <p><input type="submit" value="提交" /></p>
</form>
</body>
</html>