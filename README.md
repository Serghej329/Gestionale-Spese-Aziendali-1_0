# Gestionale delle Spese Aziendali

## Descrizione

Questo programma è un gestionale per prodotti aziendali che consente di gestire e visualizzare prodotti e categorie, generare report e analisi, e esportare i dati in formato CSV. Utilizza le librerie Newtonsoft.Json per la manipolazione dei dati JSON e Spectre.Console per l'interfaccia a riga di comando interattiva.

# Dettagli del Codice
## 1) Importazioni
Il programma inizia con l'importazione delle librerie necessarie e la definizione della classe Program contenente il metodo Main.

```csharp
using Newtonsoft.Json;
using Spectre.Console;
using System.Diagnostics;
```

## 2) Main Method
Il punto di ingresso del programma è il metodo Main. Qui vengono caricati i dati dei prodotti e delle categorie dai file JSON e viene presentato un menu principale per la navigazione.

```csharp
static void Main(string[] args)
{
    // Stampa il titolo dell'applicazione
    AnsiConsole.MarkupLine("[bold green]Gestionale delle Prodotti Aziendali[/]");

    // Percorsi dei file JSON
    string pathProdotti = @"GestioneProdotti.json";
    string pathCategorie = @"Categorie.json";

    // Carica i dati
    List<dynamic> Prodotti = CaricaProdotti(pathProdotti);
    List<string> Categorie = CaricaCategorie(pathCategorie);

    // Menu principale
    while (true)
    {
        var menu = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Scegli un'opzione:")
                .AddChoices("Gestione Prodotti", "Categorie di Prodotto", "Report e Analisi", "Ricerca Prodotto", "Esci"));

        // Switch case per gestire le scelte del menu
        switch (menu)
        {
            case "Gestione Prodotti":
                GestioneProdotti(Prodotti, Categorie, pathProdotti, pathCategorie);
                break;

            case "Categorie di Prodotto":
                VisualizzaCategorie(Prodotti, Categorie);
                break;

            case "Report e Analisi":
                VisualizzaReport(Prodotti);
                break;

            case "Ricerca Prodotto":
                RicercaProdotti(Prodotti);
                break;

            case "Esci":
                return;
        }
    }
}

```

## 3) Caricamento dei Dati
I metodi CaricaProdotti e CaricaCategorie leggono i dati da file JSON e li deserializzano in liste di oggetti dinamici e stringhe.

```csharp
static List<dynamic> CaricaProdotti(string path) { ... }
static List<string> CaricaCategorie(string path) { ... }

```

## 4) Gestione Prodotti
Il metodo GestioneProdotti offre opzioni per aggiungere, visualizzare, modificare ed eliminare prodotti.

```csharp
static void GestioneProdotti(List<dynamic> Prodotti, List<string> Categorie, string pathProdotti, string pathCategorie) { ... }
```
### Aggiunta di Prodotti
```csharp
static void AggiungiProdotto(List<dynamic> Prodotti, List<string> Categorie, string pathProdotti, string pathCategorie) { ... }

```
### Visualizzazione Prodotti
Permette di ordinare i prodotti in base a diversi criteri e di esportarli in formato CSV.
```csharp
static void VisualizzaProdotti(List<dynamic> Prodotti) { ... }
static void VisualizzaProdottiInTabella(List<dynamic> prodottiOrdinati) { ... }
static void EsportaCsvProd(List<dynamic> ProdottiOrdinati) { ... }
```
### Modifica ed Eliminazione
Consente di modificare o eliminare prodotti esistenti, aggiornando di conseguenza i file JSON.
```csharp
static void ModificaProdotto(List<dynamic> Prodotti, List<string> Categorie, string pathProdotti, string pathCategorie) { ... }
static void EliminaProdotto(List<dynamic> Prodotti, List<string> Categorie, string pathProdotti, string pathCategorie) { ... }
```
## 5) Gestione Categorie
Visualizza le categorie dei prodotti e consente la selezione e l'esportazione dei dati.
```csharp
static void VisualizzaCategorie(List<dynamic> Prodotti, List<string> Categorie) { ... }
```
## 6) Report e Analisi
Genera report su vendite mensili, per prodotto, fasce di prezzo e giorni della settimana.
```csharp
static void ReportVenditeMese(List<dynamic> Prodotti) { ... }
static void ReportVenditeProdotto(List<dynamic> Prodotti) { ... }
static void ReportVenditeFascePrezzo(List<dynamic> Prodotti) { ... }
static void ReportVenditeGiorni(List<dynamic> Prodotti) { ... }
```
## 7) Ricerca Prodotti
Funzionalità per la ricerca di prodotti specifici basati su criteri inseriti dall'utente.
```csharp
static void RicercaProdotti(List<dynamic> Prodotti) { ... }
```


## 8) Utilità Grafiche
Disegna grafici a barre per la visualizzazione dei dati.
```csharp
static void DisegnaGraficoBarre(string titolo, Dictionary<string, decimal> dati) { ... }

```

# To-Do List

- [ ] **Autenticazione Utente:**
  - Implementare un sistema di autenticazione per limitare l'accesso a utenti autorizzati.

- [ ] **Gestione Magazzino:**
  - Aggiungere funzionalità per gestire l'inventario del magazzino, includendo scorte e riordino.

- [ ] **Analisi Avanzate:**
  - Sviluppare report più avanzati come l'analisi delle tendenze di vendita e previsioni.

- [ ] **Backup e Ripristino:**
  - Aggiungere funzionalità per il backup e il ripristino dei dati in caso di errori o perdite di dati.

- [ ] **Rimuovere Ridondanze:**
  - Trovare e rimuovere eventualemente ove possibile tutte le ridonzanze del codice per accorciarlo.

- [ ] **Gestione degli Errori:**
  - Implementa una gestione degli errori robusta per evitare che l'applicazione si blocchi in caso di errori imprevisti.

- [ ] **Aggiunta Quantità:**
  - Implementa una gestione delle quantità dei prodotti disponibili.

- [ ] **Aggiunta Cliente e ordini:**
  - Implementa una gestione della ventita di uno o piu prodotti ad un cliente con le specifiche del nome categoria data e data ordine per poterlo implementare nel KPI.

- [ ] **Aggiunta Quantità:**
  - Implementa un'analisi del KPI per visualizare le vendite e le possibilità di crescità.

## Architettura del Progetto

```mermaid
---
title: Diagramma di flusso Codice Gestionale Spese Aziendali
---
stateDiagram-v2
    [*] --> MenuPrincipale

    state MenuPrincipale {
        [*] --> MostraMenuPrincipale
        MostraMenuPrincipale --> GestisciProdotti : Seleziona Gestisci Prodotti
        MostraMenuPrincipale --> GestisciCategorie : Seleziona Gestisci Categorie
        MostraMenuPrincipale --> GeneraReport : Seleziona Genera Report
        MostraMenuPrincipale --> Esci : Seleziona Esci
    }

    state GestisciProdotti {
        [*] --> MostraMenuProdotti
        MostraMenuProdotti --> AggiungiProdotto : Seleziona Aggiungi Prodotto
        MostraMenuProdotti --> VisualizzaProdotti : Seleziona Visualizza Prodotti
        MostraMenuProdotti --> ModificaProdotto : Seleziona Modifica Prodotto
        MostraMenuProdotti --> EliminaProdotto : Seleziona Elimina Prodotto
        MostraMenuProdotti --> TornaMenuPrincipale : Seleziona Torna al Menu Principale

        state AggiungiProdotto {
            [*] --> InserisciDettagli
            InserisciDettagli --> VerificaDati : Dati Inseriti
            VerificaDati --> Salvato : Dati Validi
            VerificaDati --> Errore : Dati Non Validi
            Salvato --> TornaMenuPrincipale
            Errore --> TornaMenuPrincipale
        }

        state VisualizzaProdotti {
            [*] --> MostraTabella
            MostraTabella --> EsportaCSV : Seleziona Esporta in CSV
            MostraTabella --> TornaMenuPrincipale : Seleziona Torna al Menu Principale

            state EsportaCSV {
                [*] --> GeneraCSV
                GeneraCSV --> ApriCSV : Seleziona Apri File CSV
                GeneraCSV --> TornaMenuPrincipale

                state ApriCSV {
                    [*] --> ApriFile
                    ApriFile --> TornaMenuPrincipale
                }
            }
        }

        state ModificaProdotto {
            [*] --> SelezionaProdotto
            SelezionaProdotto --> InserisciNuoviDettagli
            InserisciNuoviDettagli --> VerificaNuoviDati : Dati Inseriti
            VerificaNuoviDati --> Aggiornato : Dati Validi
            VerificaNuoviDati --> Errore : Dati Non Validi
            Aggiornato --> TornaMenuPrincipale
            Errore --> TornaMenuPrincipale
        }

        state EliminaProdotto {
            [*] --> SelezionaProdotto
            SelezionaProdotto --> RimuoviProdotto
            RimuoviProdotto --> TornaMenuPrincipale
        }
    }

    state GestisciCategorie {
        [*] --> MostraMenuCategorie
        MostraMenuCategorie --> AggiungiCategoria : Seleziona Aggiungi Categoria
        MostraMenuCategorie --> VisualizzaCategorie : Seleziona Visualizza Categorie
        MostraMenuCategorie --> TornaMenuPrincipale : Seleziona Torna al Menu Principale

        state AggiungiCategoria {
            [*] --> InserisciCategoria
            InserisciCategoria --> CategoriaAggiunta : Categoria Aggiunta
            CategoriaAggiunta --> TornaMenuPrincipale
        }

        state VisualizzaCategorie {
            [*] --> MostraCategorie
            MostraCategorie --> EsportaCSV : Seleziona Esporta in CSV
            MostraCategorie --> TornaMenuPrincipale : Seleziona Torna al Menu Principale

            state EsportaCSV {
                [*] --> GeneraCSV
                GeneraCSV --> ApriCSV : Seleziona Apri File CSV
                GeneraCSV --> TornaMenuPrincipale

                state ApriCSV {
                    [*] --> ApriFile
                    ApriFile --> TornaMenuPrincipale
                }
            }
        }
    }

    state GeneraReport {
        [*] --> MostraMenuReport
        MostraMenuReport --> VenditePerCategoria : Seleziona Vendite per Categoria
        MostraMenuReport --> VenditeMensili : Seleziona Vendite Mensili
        MostraMenuReport --> VenditePerProdotto : Seleziona Vendite per Prodotto
        MostraMenuReport --> FasceDiPrezzo : Seleziona Fasce di Prezzo
        MostraMenuReport --> VenditePerGiorno : Seleziona Vendite per Giorno della Settimana
        MostraMenuReport --> TornaMenuPrincipale : Seleziona Torna al Menu Principale
    }

    Esci --> [*]

```
