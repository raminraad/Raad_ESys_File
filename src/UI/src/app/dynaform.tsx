import { useState } from "react";
//import Script from "next/index";

import React from "react";
import App from "./App";

const Dynaform = () => {
  const [inputValue, setInputValue] = useState<string>("Next.js");

  return (
    <section className="md:flex-row items-center md:justify-between mt-16 mb-16 md:mb-12">
      <div dir="rtl" className="ltr:ml-3 rtl:mr-3">
        <h1 className="text-6xl md:text-8xl font-bold tracking-tighter leading-tight md:pr-8">
          DynaForm
        </h1>
        <div className="flex flex-col">
          <h4 className="text-lg mt-5">
            <App />A Dynamic Form with calculation
          </h4>
          <div className="flex items-center sm:flex-nowrap"></div>
        </div>
      </div>
    </section>
  );
};

export default Dynaform;
