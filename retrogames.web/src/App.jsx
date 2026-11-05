import { Route, Routes } from 'react-router';
import Layout from './components/Layout';
import RotaProtegida from './components/RotaProtegida';
import Catalogo from './pages/Catalogo';
import DetalheJogo from './pages/DetalheJogo';
import Login from './pages/Login';
import Jogos from './pages/Jogos';
import FormularioJogo from './pages/FormularioJogo';
import Plataforma from './pages/Plataforma';
import NaoEncontrada from './pages/NaoEncontrada';
export default function App() { return <Routes><Route element={<Layout />}><Route index element={<Catalogo />} /><Route path="jogos/:id" element={<DetalheJogo />} /><Route path="login" element={<Login />} /><Route element={<RotaProtegida somenteAdministrador />}><Route path="admin/jogos" element={<Jogos />} /><Route path="admin/jogos/novo" element={<FormularioJogo />} /><Route path="admin/jogos/:id/editar" element={<FormularioJogo />} /><Route path="admin/plataformas" element={<Plataforma />} /></Route><Route path="*" element={<NaoEncontrada />} /></Route></Routes>; }
