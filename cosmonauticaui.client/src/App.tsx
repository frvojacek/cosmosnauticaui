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

    async function handleDelete(id: number) {
        await fetch(`${domain}/api/Document/${id}`, {
            method: "DELETE"
        });
        fetchDocuments();
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
                    <button><svg xmlns="http://www.w3.org/2000/svg" height="24px" viewBox="0 -960 960 960" width="24px" fill="#002e3c"><path d="M480-320 280-520l56-58 104 104v-326h80v326l104-104 56 58-200 200ZM240-160q-33 0-56.5-23.5T160-240v-120h80v120h480v-120h80v120q0 33-23.5 56.5T720-160H240Z" /></svg></button>
                </a>
                <button onClick={() => handleEditDocument(document)}><svg xmlns="http://www.w3.org/2000/svg" height="24px" viewBox="0 -960 960 960" width="24px" fill="#002e3c"><path d="M200-200h57l391-391-57-57-391 391v57Zm-80 80v-170l528-527q12-11 26.5-17t30.5-6q16 0 31 6t26 18l55 56q12 11 17.5 26t5.5 30q0 16-5.5 30.5T817-647L290-120H120Zm640-584-56-56 56 56Zm-141 85-28-29 57 57-29-28Z" /></svg></button>
                <button onClick={() => handleDelete(document.id)}><svg xmlns="http://www.w3.org/2000/svg" height="24px" viewBox="0 -960 960 960" width="24px" fill="#002e3c"><path d="M280-120q-33 0-56.5-23.5T200-200v-520h-40v-80h200v-40h240v40h200v80h-40v520q0 33-23.5 56.5T680-120H280Zm400-600H280v520h400v-520ZM360-280h80v-360h-80v360Zm160 0h80v-360h-80v360ZM280-720v520-520Z" /></svg></button>
            </td>
        </tr>
    ));

    return (
        <>
            <header>
                <img className="logo" src="https://www.commerzbank.de/ms/media/favicons/CB-2022-Ribbon_RGB.svg" aria-label="Commerzbank Logo"></img>
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
                                <button id="closeButton" type="button" onClick={() => { createDialog.current?.close(); setSelectedDocument(null); }}><svg xmlns="http://www.w3.org/2000/svg" height="24px" viewBox="0 -960 960 960" width="24px" fill="#FFFFFF"><path d="m256-200-56-56 224-224-224-224 56-56 224 224 224-224 56 56-224 224 224 224-56 56-224-224-224 224Z" /></svg></button>
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
                            <button><svg xmlns="http://www.w3.org/2000/svg" height="24px" viewBox="0 -960 960 960" width="24px" fill="#002e3c"><path d="M784-120 532-372q-30 24-69 38t-83 14q-109 0-184.5-75.5T120-580q0-109 75.5-184.5T380-840q109 0 184.5 75.5T640-580q0 44-14 83t-38 69l252 252-56 56ZM380-400q75 0 127.5-52.5T560-580q0-75-52.5-127.5T380-760q-75 0-127.5 52.5T200-580q0 75 52.5 127.5T380-400Z" /></svg></button>
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