import type { Metadata } from "next";
import { Inter } from "next/font/google";
import "./globals.css";
import Header from "@/components/shared/Header";
import Script from "next/script";

const inter = Inter({ subsets: ["latin"] });

export const metadata: Metadata = {
  title: "SurveyJS + NextJS Quickstart Template",
  description: "Generated by create next app",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en">
      <head>
        <link
          href="https://unpkg.com/survey-core/defaultV2.min.css"
          type="text/css"
          rel="stylesheet"
        ></link>

        <Script
          type="text/javascript"
          src="https://unpkg.com/survey-core/survey.core.min.js"
        ></Script>
        <Script
          type="text/javascript"
          src="https://unpkg.com/survey-js-ui/survey-js-ui.min.js"
        ></Script>
        <Script type="text/javascript" src="index.js"></Script>
      </head>

      <body className={inter.className}>
        <Header />
        <div id="surveyContainer"></div>
        <main>{children}</main>
      </body>
    </html>
  );
}
