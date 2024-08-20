import { useState, useEffect, type FormEvent } from "react";

import './App.css';

interface Document {
    id: number
    name: string
    file: string
    places: string[]
    counterParties: string[]
    products: string[]
}

export enum searchTypes {
    places,
    counterParties,
    products
}

function App() {
    const [documents, setDocument] = useState<Document[]>([]);

    const domain = import.meta.env.PROD ? "" : "https://localhost:7075";

    useEffect(() => {
        fetchDocuments()
    }, [])

    async function fetchDocuments() {
        const response = await fetch(domain + "/api/Document")
        const documents = await response.json() as Document[]
        setDocument(documents)
    }
    
    async function handleSubmit(event: FormEvent) {
        event.preventDefault();
        const target = event.target as HTMLFormElement;
        await fetch(`${domain}/api/Document`, {
            method: "POST",
            body: new FormData(target)
        });
        fetchDocuments()
    }

    async function handleSearch(event: FormEvent) {
        event.preventDefault();
        const target = event.target as HTMLFormElement;
        console.debug(target.searchType.value);
        const response = await fetch(`${domain}/api/Document?` + new URLSearchParams({
            searchType: target.searchType.value,
            searchInput: target.searchInput.value
        }));

        const documents = await response.json() as Document[];
        setDocument(documents)
    }
    
    const listDocuments = documents.map((document, index) =>
        <tr key={index}>
            <td>{document.name}</td>
            <td>{document.places}</td>
            <td>{document.counterParties}</td>
            <td>{document.products}</td>
            <td>
                <a href={`${domain}/api/Document/${document}`}>
                    <button>Download</button>
                </a>
            </td>
        </tr>
    )

    return (
        <>
            <form id="document-create" onSubmit={handleSubmit}>
                <div>
                    <label htmlFor="Name">Name</label>
                    <input id="name" name="name" />
                </div>
                <div>
                    <label htmlFor="file">File</label>
                    <input id="file" name="file" type="file" />
                </div>
                <div>
                    <label htmlFor="places">Places</label>
                    <input id="places" name="places" />
                </div>
                <div>
                    <label htmlFor="counterParties">Counter Parties</label>
                    <input id="counterParties" name="counterParties" />
                </div>
                <div>
                    <label htmlFor="products">Products</label>
                    <input id="products" name="products" />
                </div>
                <button>Submit</button>
            </form>
            <form id="document-search" onSubmit={handleSearch}>
                <select id="searchType" name="searchType">
                    <option value="places">Place</option>
                    <option value="counterParties">Counter Party</option>
                    <option value="products">Product</option>
                </select>
                <input id="searchInput" name="searchInput" type="text"></input>
                <button>Search</button>
            </form>
            <table>
                <thead>
                    <td>Document Name</td>
                    <td>Places</td>
                    <td>Counter parties</td>
                    <td>Products</td>
                </thead>
                <tbody>
                    {listDocuments}
                </tbody>
            </table>
        </>
    );
}

export default App;