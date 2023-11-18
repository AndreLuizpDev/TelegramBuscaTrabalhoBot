### Tabela `Freelancer`:
- **ID**: Identificador único do freelancer.
- **UserTelegramID**: Identificação do usuário no Telegram.
- **Name**: Nome do freelancer.
- **Stacks**: Habilidades ou tecnologias em que o freelancer é especializado.
- **ExperienceTime**: Tempo de experiência do freelancer.
- **Portfolio**: Link para o portfólio do freelancer.
- **ContactTelegram**: Contato do freelancer pelo Telegram (t.me/xxxx).
- **ContactEmail**: Endereço de e-mail do freelancer.
- **ContactPhone**: Número de telefone do freelancer.
- **OtherContacts**: Outros meios de contato do freelancer.
- **Status**: Indica o status do cadastro freelancer (1 - Ativo, 0- Inativo).
- **Verified**: Indica se o perfil do freelancer foi verificado.
- **LastUpdate**: Data e hora da última atualização do perfil.
- **RegistrationDate**: Data e hora de registro do freelancer.
- **InactiveDate**: Data e hora de inatividade do freelancer.

### Tabela `Company`:
- **ID**: Identificador único da empresa.
- **UserTelegramID**: Identificação do usuário no Telegram.
- **Name**: Nome da empresa.
- **State**: Estado onde a empresa está localizada.
- **Country**: País onde a empresa está localizada.
- **ContactTelegram**: Contato da empresa pelo Telegram (t.me/xxxx).
- **ContactEmail**: Endereço de e-mail da empresa.
- **ContactPhone**: Número de telefone da empresa.
- **OtherContacts**: Outros meios de contato da empresa.
- **Status**: Indica o status da empresa (1 - Ativo, 0- Inativo).
- **Verified**: Indica se o perfil da empresa foi verificado.
- **LastUpdate**: Data e hora da última atualização do perfil da empresa.
- **RegistrationDate**: Data e hora de registro da empresa.
- **InactiveDate**: Data e hora de inatividade da empresa.

### Tabela `Vacant`:
- **ID**: Identificador único da vaga.
- **CompanyID**: ID da empresa responsável pela vaga.
- **VacantName**: Nome da vaga.
- **Description**: Descrição da vaga.
- **SkillsRequired**: Habilidades necessárias para a vaga (separado por virgula).
- **SpecialSkills**: Habilidades especiais desejadas para a vaga (separado por virgula).
- **Benefits**: Benefícios oferecidos para a vaga.
- **Modality**: Modalidade da vaga (presencial, remoto, híbrido).
- **CoverageCity**: Cidade de cobertura da vaga, se houver.
- **CoverageState**: Estado de cobertura da vaga, se houver.
- **CoverageCountry**: País de cobertura da vaga, se houver.
- **BudgetMin**: Orçamento mínimo da vaga.
- **BudgetMax**: Orçamento máximo da vaga.
- **Status**: Status da vaga (1 - Ativo, 0- Inativo).
- **CreationDate**: Data de criação da vaga.
- **LastUpdate**: Data e hora da última atualização da vaga.
- **ExclusionDate**: Data de exclusão da vaga, se aplicável.

### Tabela `FreelancerJob`:
- **ID**: Identificador único do trabalho freelancer.
- **EmployerID**: ID da empresa empregadora.
- **Description**: Descrição do trabalho freelancer.
- **ProjectType**: Tipo de projeto.
- **SkillsRequired**: Habilidades necessárias para o trabalho freelancer.
- **SpecialSkills**: Habilidades especiais desejadas para o trabalho freelancer.
- **BudgetMin**: Orçamento mínimo para o trabalho freelancer.
- **BudgetMax**: Orçamento máximo para o trabalho freelancer.
- **Status**: Status do trabalho freelancer.
- **ExpirationDate**: Data de expiração do trabalho freelancer.
- **CreationDate**: Data de criação do trabalho freelancer.
- **LastUpdate**: Data e hora da última atualização do trabalho freelancer.
- **ExclusionDate**: Data de exclusão do trabalho freelancer, se aplicável.

### Tabela `Application`:
- **ID**: Identificador único da aplicação.
- **FreelancerID**: ID do freelancer que aplicou para a vaga.
- **JobID**: ID do trabalho freelancer aplicado.
- **VacantID**: ID da vaga à qual a aplicação se refere.
- **ProposalDetails**: Detalhes da proposta do freelancer.
- **ProposedHourlyRate**: Taxa horária proposta pelo freelancer.
- **TotalHours**: Total de horas estimadas para o trabalho freelancer.
- **Status**: Status da aplicação.
- **SubmissionDate**: Data de submissão da aplicação.
- **AcceptedDate**: Data de aceitação da aplicação, se aplicável.
- **RejectedDate**: Data de rejeição da aplicação, se aplicável.
- **EmployerObservation**: Observação do empregador sobre a aplicação.

### Tabela `CompanyToFreelancerReview`:
- **ID**: Identificador único da avaliação.
- **CompanyID**: ID da empresa que realiza a avaliação.
- **FreelancerID**: ID do freelancer avaliado.
- **JobID**: ID do trabalho relacionado à avaliação.
- **Rating**: Avaliação da empresa para o freelancer.
- **Comment**: Comentário da empresa sobre o freelancer.
- **ReviewDate**: Data e hora da avaliação.

### Tabela `FreelancerToCompanyReview`:
- **ID**: Identificador único da avaliação.
- **FreelancerID**: ID do freelancer que realiza a avaliação.
- **CompanyID**: ID da empresa avaliada.
- **JobID**: ID do trabalho relacionado à avaliação.
- **Rating**: Avaliação do freelancer para a empresa.
- **Comment**: Comentário do freelancer sobre a empresa.
- **ReviewDate**: Data e hora da avaliação.