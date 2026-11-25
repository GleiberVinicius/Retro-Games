import { useEffect, useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import { jogoApi } from '../api/servicos';
import { obterMensagemErro } from '../api/erros';
import { formatarPreco, formatarDataHora } from '../lib/formato';
import { Carregando, MensagemErro } from '../components/MensagensEstado';

// Cor decorativa do topo, escolhida pelo nome da plataforma (igual à do cartão do catálogo).
function indiceDeCor(texto = '') {
  let valor = 0;
  for (const letra of texto) valor = (valor * 31 + letra.charCodeAt(0)) >>> 0;
  return valor % 6;
}

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

  const voltarAoCatalogo = <Link to="/" className="btn btn-sm btn-outline-dark">← Voltar ao catálogo</Link>;

  if (erro) return <>{voltarAoCatalogo}<div className="mt-3"><MensagemErro mensagem={erro} /></div></>;
  if (!jogo) return <Carregando texto="Carregando jogo..." />;

  const cor = `rg-cor-${indiceDeCor(jogo.plataforma)}`;
  const esgotado = !jogo.estoque;

  return <>
    {voltarAoCatalogo}
    <article className="rg-painel mt-3 overflow-hidden">
      <header className={`rg-detalhe-topo ${cor}`}>
        <div className="d-flex flex-wrap gap-2">
          <span className="badge etiqueta bg-white">{jogo.plataforma}</span>
          <span className="badge etiqueta bg-white">{jogo.condicao}</span>
        </div>
        <h1>{jogo.nome}</h1>
        <p className="mb-0 fw-semibold">{jogo.anoLancamento} · {jogo.generos.map((genero) => genero.nome).join(' / ')}</p>
      </header>
      <div className="rg-detalhe-corpo">
        <div className="row g-4">
          <div className="col-12 col-lg-7 col-xl-8">
            <h2 className="h5">Sobre o jogo</h2>
            <p className="rg-descricao mb-0">{jogo.descricao || 'Um clássico para sua coleção.'}</p>
          </div>
          <div className="col-12 col-lg-5 col-xl-4">
            <aside className="rg-detalhe-lateral" aria-label="Preço e disponibilidade">
              <div className="rg-preco fs-2 mb-2">{formatarPreco(jogo.preco)}</div>
              <p className="mb-3"><span className={`rg-situacao ${esgotado ? 'rg-situacao-esgotado' : 'rg-situacao-estoque'}`}>{esgotado ? 'Esgotado' : `${jogo.estoque} unidade(s) em estoque`}</span></p>
              <dl className="mb-0">
                <dt>Plataforma</dt><dd>{jogo.plataforma}</dd>
                <dt>Condição</dt><dd>{jogo.condicao}</dd>
                <dt>Cadastro</dt><dd className="mb-0">{formatarDataHora(jogo.dataCadastro)}</dd>
              </dl>
            </aside>
          </div>
        </div>
      </div>
    </article>
  </>;
}
