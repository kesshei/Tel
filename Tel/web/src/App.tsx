import React, { useEffect, useRef, useState } from "react";
import type { FormProps, InputRef } from "antd";
import { Flex, notification } from "antd";
import { Button, Form, Input } from "antd";
import axios from "axios";
type FieldType = {
  username?: string;
  ip?: string;
};

interface addIP {
  code: number;
}
interface IPInfo {
  code: number;
  ip: string;
}

const onFinish: FormProps<FieldType>["onFinish"] = async (values) => {
  console.log(values);
  try {
    const response = await axios.post<addIP>(
      "http://localhost:1234/api/Info/AddIP",
      values
    );
    if (response.data.code == 0) {
      // 处理后台响应，例如保存成功的提示
      notification.success({
        message: "保存成功",
        description: "数据已成功保存到后台。",
      });
      return;
    }
  } catch (rror) {
    console.log(rror);
  }
  // 处理后台错误，例如保存失败的提示
  notification.error({
    message: "保存失败",
    description: "数据保存出现问题，请稍后再试。",
  });
};

const onFinishFailed: FormProps<FieldType>["onFinishFailed"] = () => {
  notification.error({ message: "提交失败", description: "请检查网络" });
};
const App: React.FC = () => {
  const [value, setValue] = useState<string>('');
  const ipElement = useRef<InputRef>(null);
   useEffect(() => {
    axios
      .get<IPInfo>("http://localhost:1234/api/Info/IP")
      .then((response) => {
        if (response.data.code == 200) {
          setValue(response.data.ip);
          console.log(response.data.ip)
          if(ipElement.current!=null&&ipElement.current.input!=null)
          {
            ipElement.current.input.value = response.data.ip
          }
        }
      })
      .catch((error) => {
        console.error("Error fetching info:", error);
      });
  }, []);
  // const handleChange = (e: { target: { value: string}; }) => {
  //   console.log(e.target.value)
  //   setValue(e.target.value);
  // };
  return (
    <Flex className="app" justify={"center"} align={"center"}>
      <Form
        name="basic"
        labelCol={{ span: 8 }}
        wrapperCol={{ span: 16 }}
        style={{ maxWidth: 600 }}
        initialValues={{ remember: true }}
        onFinish={onFinish}
        onFinishFailed={onFinishFailed}
        autoComplete="off"
      >
        <div>
          {value}
        </div>
        <Form.Item<FieldType>
          label="ip地址"
          name="ip"
          rules={[{ required: true, message: "请输入你的本地ip!" }]}
        >
          <Input type="text" ref={ipElement}/>
        </Form.Item>
        <Form.Item<FieldType>
          label="用户名"
          name="username"
          rules={[{ required: true, message: "输入你专属的用户名!" }]}
        >
          <Input type="text" />
        </Form.Item>
        <Form.Item wrapperCol={{ offset: 8, span: 16 }}>
          <Button type="primary" htmlType="submit">
            提交
          </Button>
        </Form.Item>
      </Form>
    </Flex>
  );
};

export default App;
