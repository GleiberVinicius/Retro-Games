USE retrogames;

-- Execute cada comando de teste separadamente, selecionando apenas a instrução.
-- Os quatro primeiros devem falhar com o código indicado; o quinto deve inserir.
-- Depois de executar, anote em cada comentário o resultado observado no Workbench.

-- Teste 1: plataforma inexistente.
-- Resultado esperado: erro 1452 (violação de chave estrangeira).
-- Resultado observado: erro 1452 - Cannot add or update a child row.
INSERT INTO tb_jogo (
  no_jogo,
  nu_ano_lancamento,
  vl_preco,
  qt_estoque,
  st_ativo,
  dt_cadastro,
  co_plataforma,
  co_condicao_jogo
)
VALUES (
  '__teste_etapa5_fk__',
  2000,
  10.00,
  1,
  TRUE,
  UTC_TIMESTAMP(),
  2147483647,
  1
);

-- Teste 2: nome de plataforma já cadastrado.
-- Resultado esperado: erro 1062 (violação de UNIQUE).
-- Resultado observado: erro 1062 - Duplicate entry 'Atari 2600'.
INSERT INTO tb_plataforma (no_plataforma, no_fabricante, nu_ano_lancamento)
VALUES ('Atari 2600', 'Atari', 1977);

-- Teste 3: excluir uma plataforma que ainda possui jogos.
-- Resultado esperado: erro 1451 (violação de chave estrangeira).
-- Resultado observado: erro 1451 - Cannot delete or update a parent row.
DELETE FROM tb_plataforma
WHERE no_plataforma = 'Atari 2600';

-- Teste 4: inserir jogo sem o nome obrigatório.
-- Resultado esperado: erro 1048 (coluna no_jogo não pode ser nula).
-- Resultado observado: erro 1048 - Column 'no_jogo' cannot be null.
INSERT INTO tb_jogo (
  no_jogo,
  nu_ano_lancamento,
  vl_preco,
  qt_estoque,
  st_ativo,
  dt_cadastro,
  co_plataforma,
  co_condicao_jogo
)
SELECT
  NULL,
  2000,
  10.00,
  1,
  TRUE,
  UTC_TIMESTAMP(),
  plataforma.co_seq_plataforma,
  1
FROM tb_plataforma AS plataforma
WHERE plataforma.no_plataforma = 'Atari 2600';

-- Teste 5: preço zero. O banco aceita; a RN06 é validada na camada Service.
-- Resultado esperado: 1 linha inserida, com vl_preco = 0.00.
-- Resultado observado: executado duas vezes; 1 linha inserida em cada execução.
-- Limpeza observada: os dois registros temporários (IDs 34 e 35) foram removidos.
INSERT INTO tb_jogo (
  no_jogo,
  nu_ano_lancamento,
  vl_preco,
  qt_estoque,
  st_ativo,
  dt_cadastro,
  co_plataforma,
  co_condicao_jogo
)
SELECT
  '__teste_etapa5_preco_zero__',
  2000,
  0.00,
  1,
  TRUE,
  UTC_TIMESTAMP(),
  plataforma.co_seq_plataforma,
  1
FROM tb_plataforma AS plataforma
WHERE plataforma.no_plataforma = 'Atari 2600';

-- Confirme a linha temporária antes de removê-la.
SELECT no_jogo, vl_preco
FROM tb_jogo
WHERE no_jogo = '__teste_etapa5_preco_zero__';

-- Limpeza: remova o jogo temporário aceito pelo banco.
DELETE FROM tb_jogo
WHERE co_seq_jogo > 0
  AND no_jogo = '__teste_etapa5_preco_zero__';

-- Conferência final: ambas as consultas devem retornar zero linhas.
SELECT no_jogo
FROM tb_jogo
WHERE no_jogo IN ('__teste_etapa5_fk__', '__teste_etapa5_preco_zero__');

SELECT no_plataforma
FROM tb_plataforma
WHERE no_plataforma = '__teste_etapa5_sem_jogos__';
