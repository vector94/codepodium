import { BrowserRouter, Routes, Route } from "react-router-dom";
import Layout from "./components/Layout";
import ContestsPage from "./pages/ContestsPage";

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Layout />}>
          <Route index element={<ContestsPage />} />
        </Route>
      </Routes>
    </BrowserRouter>
  );
}
