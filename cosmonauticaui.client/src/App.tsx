import { useState, useEffect, type FormEvent, useRef } from "react";

import './App.css';

interface Document {
    id: number
    name: string
    file: string
    places: string[]
    counterParties: string[]
    products: string[]
    validFrom: string
    validTo: string
}

export enum searchTypes {
    places,
    counterParties,
    products
}

function App() {
    const [documents, setDocument] = useState<Document[]>([]);
    const [selectedDocument, setSelectedDocument] = useState<Document | null>(null);
    const createDialog = useRef<HTMLDialogElement>(null);

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
        const formData = new FormData(target);

        if (selectedDocument) {
            const file = formData.get('file') as File | null;

            if (file && file.size <= 0) {
                formData.delete("file");
            }
            await fetch(`${domain}/api/Document/${selectedDocument.id}`, {
                method: "PUT",
                body: formData
            });
        } else {
            // Create new document
            await fetch(`${domain}/api/Document`, {
                method: "POST",
                body: formData
            });
        }

        fetchDocuments();
        createDialog.current?.close();
        setSelectedDocument(null); // Reset selected document
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

    function handleEditDocument(document: Document) {
        setSelectedDocument(document);
        createDialog.current?.show();
    }
    
    const listDocuments = documents.map((document, index) => (
        <tr key={index}>
            <td>{document.name}</td>
            <td>{document.places.join(", ")}</td>
            <td>{document.counterParties.join(", ")}</td>
            <td>{document.products.join(", ")}</td>
            <td className="td_icons">
                <a href={`${domain}/api/Document/${document.id}`}>
                    <button><img src="/src/SVGs/download_icon.svg" alt="Download" /></button>
                </a>
                <button onClick={() => handleEditDocument(document)}><img src="/src/SVGs/edit_icon.svg" alt="Edit" /></button>
                <button><img src="/src/SVGs/delete_icon.svg" alt="Delete" /></button>
            </td>
        </tr>
    ));

    return (
        <>
            <header>
                <img className="logo" src="src/SVGs/CommerzBank.svg" aria-label="Commerzbank Logo"></img>
                <h1>DRIFT</h1>
            </header>
            <div className="content">
            <aside>
                <a href="/">Documents</a>
            </aside>
                <main>
                    <button id="addButton" onClick={() => createDialog.current?.show()}>New +</button>
                    <dialog ref={createDialog}>
                        <form id="document-create" onSubmit={handleSubmit}>
                            <div className="document-create-header">
                                <h2>{selectedDocument ? "Edit document" : "New document"}</h2>
                                <button id="closeButton" type="button" onClick={() => { createDialog.current?.close(); setSelectedDocument(null); }}><img src="src/SVGs/close_icon.svg" alt="Close" /></button>
                            </div>
                            <div className="metadata">
                                <label htmlFor="Name">Name</label>
                                <input id="name" name="name" defaultValue={selectedDocument?.name || ""} required />

                                <label htmlFor="places">Places</label>
                                <input id="places" name="places" defaultValue={selectedDocument?.places.join(", ") || ""} required />

                                <label htmlFor="counterParties">Counter Parties</label>
                                <input id="counterParties" name="counterParties" defaultValue={selectedDocument?.counterParties.join(", ") || ""} required />

                                <label htmlFor="products">Products</label>
                                <input id="products" name="products" defaultValue={selectedDocument?.products.join(", ") || ""} required />

                                <label htmlFor="validFrom">Valid From</label>
                                <input id="validFrom" name="validFrom" defaultValue={selectedDocument?.validFrom || ""} required />

                                <label htmlFor="validTo">Valid To</label>
                                <input id="validTo" name="validTo" defaultValue={selectedDocument?.validTo || ""} required />
                            </div>
                            <div className="fileInput">
                                <label htmlFor="file">File</label>
                                <input id="file" name="file" type="file" required={!selectedDocument} />
                            </div>
                            <button type="submit">{selectedDocument ? "Update" : "Submit"}</button>
                        </form>
                    </dialog>
            <div id="search">
            <form id="document-search" onSubmit={handleSearch}>
                <select id="searchType" name="searchType">
                    <option value="places">Place</option>
                    <option value="counterParties">Counter Party</option>
                    <option value="products">Product</option>
                </select>
                <input id="searchInput" name="searchInput" type="text"></input>
                <button><img src="/src/SVGs/search_icon.svg"></img></button>
            </form>
            <table>
                <thead>
                    <tr>
                    <th>Document Name</th>
                    <th>Places</th>
                    <th>Counter parties</th>
                    <th>Products</th>
                    <th></th>
                    </tr>
                </thead>
                <tbody>
                    {listDocuments}
                </tbody>
                </table>
            </div>
            </main>
            </div>
        </>
    );
}

export default App;