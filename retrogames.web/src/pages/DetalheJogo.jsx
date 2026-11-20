import { useEffect, useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import { jogoApi } from '../api/servicos';
import { obterMensagemErro } from '../api/erros';
import { formatarPreco, formatarDataHora } from '../lib/formato';
import { Carregando, MensagemErro } from '../components/MensagensEstado';

export default function DetalheJogo() {
  const { id } = useParams();
  const [jogo, setJogo] = useState(null);
  const [erro, setErro] = useState('');

  useEffect(() => {
    let requisicaoAtiva = true;
    setJogo(null);
    setErro('');

    jogoApi.buscarPorId(id)
      .then((resultado) => {
        if (requisicaoAtiva) setJogo(resultado);
      })
      .catch((falha) => {
        if (requisicaoAtiva) setErro(obterMensagemErro(falha));
      });

    return () => {
      requisicaoAtiva = false;
    };
  }, [id]);

  const voltarAoCatalogo = <Link to="/" className="text-decoration-none">← Voltar ao catálogo</Link>;

  if (erro) return <>{voltarAoCatalogo}<div className="mt-3"><MensagemErro mensagem={erro} /></div></>;
  if (!jogo) return <Carregando texto="Carregando jogo..." />;

  return <>
    {voltarAoCatalogo}
    <article className="card border-0 shadow-sm mt-3">
      <div className="card-body p-4 p-lg-5">
        <div className="d-flex gap-2 mb-3">
          <span className="badge etiqueta">{jogo.plataforma}</span>
          <span className="badge text-bg-light">{jogo.condicao}</span>
        </div>
        <h1 className="display-5 fw-bold">{jogo.nome}</h1>
        <p className="text-secondary">{jogo.anoLancamento} · {jogo.generos.map((genero) => genero.nome).join(' / ')}</p>
        <p className="my-4">{jogo.descricao || 'Um clássico para sua coleção.'}</p>
        <div className="preco mb-2">{formatarPreco(jogo.preco)}</div>
        <p>{jogo.estoque ? `${jogo.estoque} unidade(s) em estoque` : 'Esgotado'}</p>
        <small className="text-secondary">Cadastro: {formatarDataHora(jogo.dataCadastro)}</small>
      </div>
    </article>
  </>;
}
