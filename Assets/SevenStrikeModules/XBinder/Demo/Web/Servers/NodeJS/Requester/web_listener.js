const express = require('express');
const fs = require('fs');
const path = require('path');
const bodyParser = require('body-parser');

const app = express();

// ---------------------------------------------------------------------------
// 常量配置
// ---------------------------------------------------------------------------
// ANSI 颜色码
const C = {
  reset: '\x1b[0m',
  red: '\x1b[31m',
  green: '\x1b[32m',
  yellow: '\x1b[33m',
  blue: '\x1b[34m',
  magenta: '\x1b[35m',
  cyan: '\x1b[36m',
  gray: '\x1b[90m',
  bold: '\x1b[1m',
};
const PORT = 9090;
const Dir_Json = path.join(__dirname, 'datas');

// ---------------------------------------------------------------------------
// 请求计数器
// ---------------------------------------------------------------------------
let requestCount = 0; // 总请求次数
const fileRequestCount = {}; // 每个 indexCode 被请求的次数

// ---------------------------------------------------------------------------
// 中间件
// ---------------------------------------------------------------------------
// 解析 JSON 请求体
app.use(bodyParser.json());
// 解析表单请求体
app.use(bodyParser.urlencoded({ extended: true }));

// ---------------------------------------------------------------------------
// 路由
// ---------------------------------------------------------------------------

/**
 * POST /data
 * 根据 indexCode 返回 public 目录下对应的 json 文件内容
 */
app.post('/data', (req, res) => {
  const { indexCode } = req.body;

  // 参数校验
  if (!indexCode) {
    return res.status(400).send('缺少参数: indexCode');
  }

  // 防止路径穿越：只允许字母、数字、下划线、短横线
  if (!/^[\w-]+$/.test(indexCode)) {
    return res.status(400).send('参数 indexCode 非法');
  }

  const jsonPath = path.join(Dir_Json, `${indexCode}.json`);

  fs.readFile(jsonPath, 'utf8', (err, data) => {
    if (err) {
      console.error(`[data] 读取失败 ${jsonPath}:`, err);
      return res.status(500).send('读取文件失败: ' + err.message);
    }

    // ---- 计数 ----
    requestCount += 1;
    fileRequestCount[indexCode] = (fileRequestCount[indexCode] || 0) + 1;

    console.log();
    console.log(
      `${C.gray}[网络请求] ------->> Json文件 = ${C.green}${indexCode}` +
      `${C.gray}, 内容长度=${C.yellow}${data.length}` +
      `${C.gray}, 总请求次数=${C.yellow}${requestCount}` +
      `${C.gray}, 该文件被请求=${C.cyan}${fileRequestCount[indexCode]}${C.gray}次${C.reset}`
    );
    console.log(`${C.reset}${data}`);
    res.type('application/json').send(data);
  });
});

// ---------------------------------------------------------------------------
// 启动服务
// ---------------------------------------------------------------------------
const server = app.listen(PORT, () => {
  const { address, port } = server.address();

  console.log();
  console.log(`${C.yellow}Node 网络服务已启动！${C.reset}`);
  console.log();
  console.log(`${C.gray}网络请求地址: ${C.reset}${C.green}http://localhost:${port}${C.reset}`);
  console.log();
  console.log(`${C.gray}请通过该路径来请求数据${C.reset} ${C.green}http://localhost:${port}/data${C.reset}`);
  console.log();
  console.log(`${C.gray}在您网络请求的参数中填写${C.reset} "${C.yellow}目标地址${C.reset}" + "${C.yellow}目标Json名称${C.reset}"`);
  console.log();
  console.log(`------------->>`);
  console.log();
});