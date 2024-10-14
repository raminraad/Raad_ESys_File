import Head from "next/head";
import Dynaform from "./dynaform";
import Layout from "./layout";

const Index = () => {
  const title = `Next.js Example with Esys.Api`;

  return (
    <>
      <Layout>
        <Head>
          <title>{title}</title>
        </Head>
        <Dynaform />
      </Layout>
    </>
  );
};

export default Index;
